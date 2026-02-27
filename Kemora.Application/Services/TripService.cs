using AutoMapper;
using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepo;
        private readonly IPlaceRepository _placeRepo;
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        public TripService(ITripRepository tripRepo, IPlaceRepository placeRepo, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _tripRepo = tripRepo;
            _placeRepo = placeRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TripDetailDto> CreateAsync(string userId, CreateTripDto dto)
        {
            var trip = new Trip
            {
                Name = dto.Name, Description = dto.Description,
                StartDate = dto.StartDate, EndDate = dto.EndDate, UserID = userId
            };
            await _tripRepo.AddAsync(trip);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<TripDetailDto>(trip);
        }

        public async Task<PagedResult<TripListDto>> ListAsync(string userId, int page, int pageSize)
        {
            var trips = await _tripRepo.GetByUserIdAsync(userId, page, pageSize);
            var count = await _tripRepo.GetCountByUserIdAsync(userId);
            return new PagedResult<TripListDto>
            {
                Items = _mapper.Map<List<TripListDto>>(trips),
                TotalCount = count,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<TripDetailDto?> GetAsync(string userId, int id)
        {
            var trip = await _tripRepo.GetWithPlacesAsync(id);
            if (trip == null || trip.UserID != userId) return null;
            return _mapper.Map<TripDetailDto>(trip);
        }

        public async Task<bool> UpdateAsync(string userId, int id, UpdateTripDto dto)
        {
            var trip = await _tripRepo.GetByIdAsync(id);
            if (trip == null || trip.UserID != userId) return false;

            if (dto.Name != null) trip.Name = dto.Name;
            if (dto.Description != null) trip.Description = dto.Description;
            if (dto.StartDate.HasValue) trip.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) trip.EndDate = dto.EndDate.Value;
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, int id)
        {
            var trip = await _tripRepo.GetByIdAsync(id);
            if (trip == null || trip.UserID != userId) return false;
            
            _tripRepo.Remove(trip);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<TripPlaceResponseDto?> AddPlaceAsync(string userId, int tripId, AddTripPlaceDto dto)
        {
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null || trip.UserID != userId) return null;

            var place = await _placeRepo.GetByIdAsync(dto.PlaceID);
            if (place == null) return null;

            if (await _tripRepo.TripPlaceExistsAsync(tripId, dto.PlaceID)) return null;

            var tp = new TripPlace { TripID = tripId, PlaceID = dto.PlaceID, VisitDate = dto.VisitDate, Notes = dto.Notes };
            await _tripRepo.AddTripPlaceAsync(tp);
            await _unitOfWork.CommitAsync();

            var resp = _mapper.Map<TripPlaceResponseDto>(tp);
            resp.PlaceName = place.Name;
            return resp;
        }

        public async Task<bool> UpdatePlaceAsync(string userId, int tripId, int tpId, UpdateTripPlaceDto dto)
        {
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null || trip.UserID != userId) return false;

            var tp = await _tripRepo.GetTripPlaceAsync(tpId);
            if (tp == null || tp.TripID != tripId) return false;

            if (dto.VisitDate.HasValue) tp.VisitDate = dto.VisitDate.Value;
            if (dto.Notes != null) tp.Notes = dto.Notes;
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> RemovePlaceAsync(string userId, int tripId, int tpId)
        {
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null || trip.UserID != userId) return false;

            var tp = await _tripRepo.GetTripPlaceAsync(tpId);
            if (tp == null || tp.TripID != tripId) return false;

            _tripRepo.RemoveTripPlace(tp);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
