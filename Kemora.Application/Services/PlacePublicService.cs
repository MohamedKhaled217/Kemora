using AutoMapper;
using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class PlacePublicService : IPlacePublicService
    {
        private readonly IPlaceRepository _placeRepo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public PlacePublicService(IPlaceRepository placeRepo, IMapper mapper, ICacheService cacheService)
        {
            _placeRepo = placeRepo;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<PlacePublicDto>> GetPlacesAsync(int? governorateId, int? categoryId, string? searchQuery, int page, int pageSize)
        {
            var places = await _placeRepo.GetFilteredAsync(searchQuery, governorateId, categoryId, page, pageSize);
            var count = await _placeRepo.GetFilteredCountAsync(searchQuery, governorateId, categoryId);
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

            var dto = _mapper.Map<PlaceDetailPublicDto>(place);
            var activeEvents = place.Events;
            dto.ActiveEvents = _mapper.Map<List<EventResponseDto>>(activeEvents);

            _cacheService.Set(cacheKey, dto, System.TimeSpan.FromMinutes(10));
            return dto;
        }
    }
}
