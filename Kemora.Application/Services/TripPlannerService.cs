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
        private readonly IPlaceService _overpassService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        private readonly IWikipediaService _wikipedia;

        public TripPlannerService(IPlaceService overpassService, IUnitOfWork unitOfWork, ICacheService cache, IWikipediaService wikipedia)
        {
            _overpassService = overpassService;
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

            // 3. Smart Fetching Strategy: Local DB first, fallback to Overpass External API
            List<FetchedPlaceDto> finalPlaces = new List<FetchedPlaceDto>();

            if (!string.IsNullOrEmpty(request.Location))
            {
                var localPlaces = await _unitOfWork.Repository<Place>()
                    .FindAsync(p => p.Governorate != null && p.Governorate.Name == request.Location);

                finalPlaces = localPlaces.Select(p => new Kemora.Domain.Models.FetchedPlaceDto
                {
                    Name = p.Name,
                    Latitude = (double)p.Latitude,
                    Longitude = (double)p.Longitude,
                    Types = new List<string> { "db_place" }
                }).ToList();
            }

            if (finalPlaces.Count < 5)
            {
                // We don't have enough rich local data, trigger external API
                var overpassPlaces = await _overpassService.FetchNearbyPlacesAsync(
                    request.Latitude, request.Longitude,
                    request.MinRadiusKm, request.MaxRadiusKm);

                finalPlaces.AddRange(overpassPlaces);

                // Persist new places from the external API to the DB for future cached use
                foreach (var fp in overpassPlaces)
                {
                    var existsInDb = (await _unitOfWork.Repository<Place>()
                        .FindAsync(p => p.Name == fp.Name)).Any();

                    if (!existsInDb)
                    {
                        var newPlace = new Place
                        {
                            Name = fp.Name,
                            Latitude = (decimal)fp.Latitude,
                            Longitude = (decimal)fp.Longitude,
                            MainImageURL = fp.ImageUrl,
                            // GovernorateID and PlaceTypeID left null until enriched later
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

            // 4. Send to Gemini / External API
            var tripPlanContent = await _overpassService.GenerateTripPlanAsync(
                finalPlaces,
                request.DurationDays,
                request.Budget,
                request.Location,
                request.TourismTypes,
                request.Preferences); 

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
            return await _overpassService.SwapPlaceAsync(currentPlaceName, preferences);
        }
    }
}
