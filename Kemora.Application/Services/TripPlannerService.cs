using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kemora.Application.Services
{
    public class TripPlannerService : ITripPlannerService
    {
        private readonly IPlaceService _placeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        private readonly IWikipediaService _wikipedia;

        public TripPlannerService(IPlaceService placeService, IUnitOfWork unitOfWork, ICacheService cache, IWikipediaService wikipedia)
        {
            _placeService = placeService;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _wikipedia = wikipedia;
        }

        public async Task<TripPlanResponseDto> GenerateTripPlanAsync(TripPlanRequestDto request)
        {
            if (request.MinRadiusKm >= request.MaxRadiusKm)
                throw new ArgumentException("MinRadiusKm must be less than MaxRadiusKm.");

            // 1. Redis / Memory Cache handling honoring User Preferences and AlternativeIndex
            string prefs = request.Preferences ?? "none";
            string cacheKey = $"plan_{request.CenterPlaceId}_{request.Latitude}_{request.Longitude}_{request.DurationDays}_{request.Location}_{prefs}_{request.AlternativeIndex}";

            var cachedPlan = _cache.Get<TripPlanResponseDto>(cacheKey);
            if (cachedPlan != null)
                return cachedPlan;

            // 2. Center around specific landmark if provided
            if (request.CenterPlaceId.HasValue && request.CenterPlaceId > 0)
            {
                var centerPlace = await _unitOfWork.Repository<Place>().GetByIdAsync(request.CenterPlaceId.Value);
                if (centerPlace != null)
                {
                    request.Latitude = (double)centerPlace.Latitude;
                    request.Longitude = (double)centerPlace.Longitude;
                }
            }

            // 3. Smart Fetching Strategy: Local DB first
            var allDbPlaces = await _unitOfWork.Repository<Place>().GetAllAsync();
            var localPlaces = allDbPlaces
                .Select(p => new FetchedPlaceDto
                {
                    Name = p.Name,
                    Latitude = (double)p.Latitude,
                    Longitude = (double)p.Longitude,
                    Address = p.Address ?? "",
                    ImageUrl = p.MainImageURL ?? "",
                    Rating = (double)p.Rating,
                    PriceLevel = p.PriceLevel.ToString(),
                    Types = new List<string> { "db_source" },
                    DistanceKm = Math.Round(HaversineKm(request.Latitude, request.Longitude, (double)p.Latitude, (double)p.Longitude), 2)
                })
                .Where(p => p.DistanceKm <= request.MaxRadiusKm)
                .OrderBy(p => p.DistanceKm)
                .ToList();

            List<FetchedPlaceDto> finalPlaces = new List<FetchedPlaceDto>(localPlaces);

            if (finalPlaces.Count < 15)
            {
                // We don't have enough rich local data (~15), trigger external API to fill the gaps
                var googlePlaces = await _placeService.FetchNearbyPlacesAsync(
                    request.Latitude, request.Longitude,
                    request.MinRadiusKm, request.MaxRadiusKm);

                // Merge Google results, avoiding duplicates with DB places
                foreach (var gp in googlePlaces)
                {
                    if (!finalPlaces.Any(lp => lp.Name.Equals(gp.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        finalPlaces.Add(gp);

                        // Async persist to DB with RICH ATTRIBUTES
                        var newPlace = new Place
                        {
                            Name = gp.Name,
                            Address = gp.Address,
                            Latitude = (decimal)gp.Latitude,
                            Longitude = (decimal)gp.Longitude,
                            Rating = (decimal)(gp.Rating ?? 0),
                            PriceLevel = int.TryParse(gp.PriceLevel, out int pl) ? pl : 0,
                            MainImageURL = gp.ImageUrl,
                            Source = "google",
                            LastEnrichedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.Repository<Place>().AddAsync(newPlace);
                    }
                }
                await _unitOfWork.CommitAsync();
            }

            // --- FETCH IMAGES FROM WIKIPEDIA CONCURRENTLY ---
            var imageTasks = finalPlaces.Take(30).Select(async p => 
            {
                if (string.IsNullOrEmpty(p.ImageUrl))
                {
                    p.ImageUrl = await _wikipedia.GetImageUrlAsync(p.Name);
                }
            });
            await Task.WhenAll(imageTasks);

            if (finalPlaces.Count == 0)
            {
                return new TripPlanResponseDto
                {
                    TotalPlacesFound = 0,
                    Places = [],
                    TripPlan = "No places found in the specified area. Try enlarging the radius."
                };
            }

            // 4. Send to AI (WisdomGate)
            var tripPlanContent = await _placeService.GenerateTripPlanAsync(
                finalPlaces,
                request.DurationDays,
                request.Budget,
                request.Location,
                request.TourismTypes,
                request.Preferences); 

            // Clean response content (WisdomGate usually returns clean JSON if asked, but let's be safe)
            tripPlanContent = tripPlanContent.Trim();
            if (tripPlanContent.StartsWith("```"))
            {
                var startIdx = tripPlanContent.IndexOf('\n') + 1;
                var endIdx = tripPlanContent.LastIndexOf("```");
                if (endIdx > startIdx)
                {
                    tripPlanContent = tripPlanContent.Substring(startIdx, endIdx - startIdx).Trim();
                }
            }

            // Try to re-inject image URLs into the itinerary
            try
            {
                var jObject = System.Text.Json.Nodes.JsonObject.Parse(tripPlanContent);
                var itinerary = jObject?["itinerary"]?.AsArray();
                if (itinerary != null)
                {
                    foreach (var day in itinerary)
                    {
                        var activities = day?["activities"]?.AsArray();
                        if (activities != null)
                        {
                            foreach (var act in activities)
                            {
                                var placeName = act?["place"]?.ToString();
                                if (!string.IsNullOrEmpty(placeName))
                                {
                                    var match = finalPlaces.FirstOrDefault(p =>
                                        p.Name.Equals(placeName, StringComparison.OrdinalIgnoreCase));
                                    if (match != null && !string.IsNullOrEmpty(match.ImageUrl))
                                    {
                                        act["image_url"] = match.ImageUrl;
                                    }
                                }
                            }
                        }
                    }
                }
                tripPlanContent = jObject?.ToJsonString() ?? tripPlanContent;
            }
            catch { /* Ignore image injection errors — the clean JSON is still valid */ }

            var response = new TripPlanResponseDto
            {
                TotalPlacesFound = finalPlaces.Count,
                Places = finalPlaces,
                TripPlan = tripPlanContent
            };

            // 5. Save the generated alternative in Cache for instant re-loads
            _cache.Set(cacheKey, response, TimeSpan.FromHours(24));

            return response;
        }

        public async Task<string> SwapPlaceAsync(string currentPlaceName, string preferences)
        {
            return await _placeService.SwapPlaceAsync(currentPlaceName, preferences);
        }

        private static double HaversineKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLng = ToRad(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                  * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;
    }
}
