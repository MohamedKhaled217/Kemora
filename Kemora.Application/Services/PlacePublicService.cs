using AutoMapper;
using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class PlacePublicService : IPlacePublicService
    {
        private readonly IPlaceRepository _placeRepo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWikipediaService _wikipediaService;
        private readonly IPlaceService _googleService;

        public PlacePublicService(IPlaceRepository placeRepo, IMapper mapper, ICacheService cacheService, IUnitOfWork unitOfWork, IWikipediaService wikipediaService, IPlaceService googleService)
        {
            _placeRepo = placeRepo;
            _mapper = mapper;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
            _wikipediaService = wikipediaService;
            _googleService = googleService;
        }

        public async Task<PagedResult<PlacePublicDto>> GetPlacesAsync(int? governorateId, int? categoryId, string? categoryName, string? searchQuery, int page, int pageSize)
        {
            var places = await _placeRepo.GetFilteredAsync(searchQuery, governorateId, categoryId, categoryName, page, pageSize);
            
            // If fewer than 20 places found locally, trigger hydration to achieve the target density
            if (places.Count() < 20 && governorateId.HasValue && string.IsNullOrEmpty(searchQuery))
            {
                await HydrateGovernoratePlacesAsync(governorateId.Value);
                // Re-fetch after hydration
                places = await _placeRepo.GetFilteredAsync(searchQuery, governorateId, categoryId, categoryName, page, pageSize);
            }

            await EnrichPlacesWithImagesAsync(places);
            var count = await _placeRepo.GetFilteredCountAsync(searchQuery, governorateId, categoryId, categoryName);
            return new PagedResult<PlacePublicDto>
            {
                Items = _mapper.Map<List<PlacePublicDto>>(places),
                TotalCount = count,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<PlaceDetailPublicDto?> GetPlaceDetailAsync(int id)
        {
            var cacheKey = $"place_detail_{id}";
            var cached = _cacheService.Get<PlaceDetailPublicDto>(cacheKey);

            if (cached != null)
                return cached;

            var place = await _placeRepo.GetWithDetailsAsync(id);
            if (place == null) return null;

            await EnrichPlacesWithImagesAsync(new[] { place });

            var dto = _mapper.Map<PlaceDetailPublicDto>(place);
            var activeEvents = place.Events;
            dto.ActiveEvents = _mapper.Map<List<EventResponseDto>>(activeEvents);

            _cacheService.Set(cacheKey, dto, System.TimeSpan.FromMinutes(10));
            return dto;
        }

        public async Task<List<GovernorateDto>> GetGovernoratesAsync()
        {
            var cacheKey = "all_governorates";
            var cached = _cacheService.Get<List<GovernorateDto>>(cacheKey);
            if (cached != null) return cached;

            var governorates = await _unitOfWork.Repository<Governorate>().GetAllAsync();
            var dtos = _mapper.Map<List<GovernorateDto>>(governorates);

            _cacheService.Set(cacheKey, dtos, System.TimeSpan.FromHours(1));
            return dtos;
        }

        public async Task<List<PlacePublicDto>> GetTopPlacesAsync()
        {
            var cacheKey = "top_places";
            var cached = _cacheService.Get<List<PlacePublicDto>>(cacheKey);
            if (cached != null) return cached;

            var places = await _placeRepo.GetTopPlacesAsync(20);
            await EnrichPlacesWithImagesAsync(places);
            var dtos = _mapper.Map<List<PlacePublicDto>>(places);

            _cacheService.Set(cacheKey, dtos, System.TimeSpan.FromMinutes(30));
            return dtos;
        }

        private async Task HydrateGovernoratePlacesAsync(int governorateId)
        {
            var governorate = await _unitOfWork.Repository<Governorate>().GetByIdAsync(governorateId);
            if (governorate == null || governorate.Latitude == 0) return;

            // Fetch a larger volume of high-quality places (50km radius for regional coverage)
            var googleResults = await _googleService.FetchNearbyPlacesAsync((double)governorate.Latitude, (double)governorate.Longitude, 0, 50);
            
            // Get a HashSet of existing IDs to skip duplicates efficiently
            var existingPlaces = await _placeRepo.GetFilteredAsync(null, governorateId, null, null, 1, 1000);
            var existingIds = existingPlaces.Select(p => p.GooglePlaceID).Where(id => id != null).ToHashSet();
            
            // TRACKER: Ensure we add at least 20 new places
            int newAddsCount = 0;
            
            // Process a large candidate pool (100) until we hit 20 new successes
            foreach (var basicPlace in googleResults.Where(r => !existingIds.Contains(r.GooglePlaceId)).Take(100))
            {
                if (newAddsCount >= 20) break;
                if (string.IsNullOrEmpty(basicPlace.GooglePlaceId)) continue;

                // Call Place Details to get rich metadata and confirm photo availability
                var details = await _googleService.GetPlaceDetailsAsync(basicPlace.GooglePlaceId);
                
                // QUALITY CHECK: Must have at least one photo and a name
                if (details != null && !string.IsNullOrEmpty(details.ImageUrl) && !string.IsNullOrEmpty(details.Name))
                {
                    // Final safety check and add
                    var newPlace = new Place
                    {
                        GooglePlaceID = basicPlace.GooglePlaceId,
                        Name = details.Name,
                        Address = details.Address,
                        Latitude = (decimal)details.Latitude,
                        Longitude = (decimal)details.Longitude,
                        Rating = (decimal)(details.Rating ?? 0),
                        PriceLevel = int.TryParse(details.PriceLevel, out int pl) ? pl : 0,
                        MainImageURL = details.ImageUrl,
                        Phone = details.Phone,
                        Website = details.Website,
                        GovernorateID = governorateId,
                        Source = "google_sync",
                        LastEnrichedAt = System.DateTime.UtcNow
                    };
                    await _unitOfWork.Repository<Place>().AddAsync(newPlace);
                    existingIds.Add(basicPlace.GooglePlaceId); 
                    newAddsCount++;
                }
            }
            await _unitOfWork.CommitAsync();
        }

        private async Task EnrichPlacesWithImagesAsync(IEnumerable<Place> places)
        {
            bool hasUpdates = false;
            foreach (var place in places)
            {
                // If rating is 0 or image is missing, attempt to enrich from Google
                if (place.Rating == 0 || string.IsNullOrEmpty(place.MainImageURL))
                {
                    FetchedPlaceDto? match = null;

                    if (!string.IsNullOrEmpty(place.GooglePlaceID))
                    {
                        match = await _googleService.GetPlaceDetailsAsync(place.GooglePlaceID);
                    }
                    else
                    {
                        var results = await _googleService.FetchNearbyPlacesAsync((double)place.Latitude, (double)place.Longitude, 0, 5);
                        match = results.FirstOrDefault(r => 
                            r.Name.Contains(place.Name, StringComparison.OrdinalIgnoreCase) || 
                            place.Name.Contains(r.Name, StringComparison.OrdinalIgnoreCase));
                    }

                    if (match != null)
                    {
                        if (place.Rating == 0) place.Rating = (decimal)(match.Rating ?? 0);
                        if (string.IsNullOrEmpty(place.Address)) place.Address = match.Address;
                        if (!string.IsNullOrEmpty(match.ImageUrl)) place.MainImageURL = match.ImageUrl;
                        if (string.IsNullOrEmpty(place.Website)) place.Website = match.Website;
                        if (string.IsNullOrEmpty(place.Phone)) place.Phone = match.Phone;

                        _unitOfWork.Repository<Place>().Update(place);
                        hasUpdates = true;
                    }
                    
                    // Fallback to Wikipedia if no image found on Google
                    if (string.IsNullOrEmpty(place.MainImageURL))
                    {
                        var imageUrl = await _wikipediaService.GetImageUrlAsync(place.Name);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            place.MainImageURL = imageUrl;
                            _unitOfWork.Repository<Place>().Update(place);
                            hasUpdates = true;
                        }
                    }
                }
            }
            if (hasUpdates)
            {
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
