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
        private readonly IUnitOfWork _unitOfWork;

        public PlacePublicService(IPlaceRepository placeRepo, IMapper mapper, ICacheService cacheService, IUnitOfWork unitOfWork)
        {
            _placeRepo = placeRepo;
            _mapper = mapper;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<PlacePublicDto>> GetPlacesAsync(int? governorateId, int? categoryId, string? categoryName, string? searchQuery, int page, int pageSize)
        {
            var places = await _placeRepo.GetFilteredAsync(searchQuery, governorateId, categoryId, categoryName, page, pageSize);
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
            var dtos = _mapper.Map<List<PlacePublicDto>>(places);

            _cacheService.Set(cacheKey, dtos, System.TimeSpan.FromMinutes(30));
            return dtos;
        }
    }
}
