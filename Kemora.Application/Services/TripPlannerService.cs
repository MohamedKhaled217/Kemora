using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class TripPlannerService : ITripPlannerService
    {
        private readonly IPlaceService _overpassService;

        public TripPlannerService(IPlaceService overpassService)
        {
            _overpassService = overpassService;
        }

        public async Task<TripPlanResponseDto> GenerateTripPlanAsync(TripPlanRequestDto request)
        {
            if (request.MinRadiusKm >= request.MaxRadiusKm)
                throw new ArgumentException("MinRadiusKm must be less than MaxRadiusKm.");

            var places = await _overpassService.FetchNearbyPlacesAsync(
                request.Latitude, request.Longitude,
                request.MinRadiusKm, request.MaxRadiusKm);

            if (places.Count == 0)
            {
                return new TripPlanResponseDto
                {
                    TotalPlacesFound = 0,
                    Places = [],
                    TripPlan = "No places found in the specified area. Try enlarging the radius."
                };
            }

            var tripPlan = await _overpassService.GenerateTripPlanAsync(
                places,
                request.DurationDays,
                request.Budget,
                request.Location,
                request.TourismTypes,
                request.Preferences);

            return new TripPlanResponseDto
            {
                TotalPlacesFound = places.Count,
                Places = places,
                TripPlan = tripPlan
            };
        }
    }
}
