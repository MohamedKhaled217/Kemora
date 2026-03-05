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
        private readonly IRepository<TripPlace> _tripPlaceRepo;
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        public TripService(ITripRepository tripRepo, IPlaceRepository placeRepo, IRepository<TripPlace> tripPlaceRepo, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _tripRepo = tripRepo;
            _placeRepo = placeRepo;
            _tripPlaceRepo = tripPlaceRepo;
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

            if (await _tripPlaceRepo.AnyAsync(tp => tp.TripID == tripId && tp.PlaceID == dto.PlaceID)) return null;

            var tp = new TripPlace { TripID = tripId, PlaceID = dto.PlaceID, VisitDate = dto.VisitDate, Notes = dto.Notes };
            await _tripPlaceRepo.AddAsync(tp);
            await _unitOfWork.CommitAsync();

            var resp = _mapper.Map<TripPlaceResponseDto>(tp);
            resp.PlaceName = place.Name;
            return resp;
        }

        public async Task<bool> UpdatePlaceAsync(string userId, int tripId, int tpId, UpdateTripPlaceDto dto)
        {
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null || trip.UserID != userId) return false;

            var tp = await _tripPlaceRepo.GetByIdAsync(tpId);
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

            var tp = await _tripPlaceRepo.GetByIdAsync(tpId);
            if (tp == null || tp.TripID != tripId) return false;

            _tripPlaceRepo.Remove(tp);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<TripDetailDto> SaveAIPlanAsync(string userId, SaveAIPlanDto dto)
        {
            var trip = new Trip
            {
                Name = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UserID = userId
            };
            await _tripRepo.AddAsync(trip);
            await _unitOfWork.CommitAsync(); // Get TripID

            foreach (var act in dto.Activities)
            {
                // Try to find if the place already exists by name and proximity
                var existingPlace = (await _placeRepo.FindAsync(p => p.Name == act.Name)).FirstOrDefault();
                
                int placeId;
                if (existingPlace == null)
                {
                    // Create a new place if it doesn't exist
                    var newPlace = new Place
                    {
                        Name = act.Name,
                        Description = act.Description ?? string.Empty,
                        Latitude = (decimal)act.Latitude,
                        Longitude = (decimal)act.Longitude,
                        MainImageURL = act.ImageUrl ?? string.Empty,
                        PlaceTypeID = 1 // Default to 1 (usually "Other" or "Tourist Attraction" in typical seeders)
                        // Note: ideally we'd map Governorate here, but for simplicity we'll let it be null or handle via geocoding later
                    };
                    await _placeRepo.AddAsync(newPlace);
                    await _unitOfWork.CommitAsync();
                    placeId = newPlace.PlaceID;
                }
                else
                {
                    placeId = existingPlace.PlaceID;
                }

                var tp = new TripPlace
                {
                    TripID = trip.TripID,
                    PlaceID = placeId,
                    VisitDate = act.VisitDate,
                    Notes = act.Notes
                };
                await _tripPlaceRepo.AddAsync(tp);
            }

            await _unitOfWork.CommitAsync();
            return await GetAsync(userId, trip.TripID) ?? _mapper.Map<TripDetailDto>(trip);
        }
    }
}
