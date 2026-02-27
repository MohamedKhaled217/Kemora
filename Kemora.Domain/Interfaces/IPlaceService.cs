using Kemora.Domain.Enums;
using Kemora.Domain.Models;

namespace Kemora.Domain.Interfaces
{
    public interface IPlaceService
    {
        /// <summary>
        /// Fetches nearby places from OpenStreetMap within a radius band [minRadiusKm, maxRadiusKm].
        /// Searches for hotels, restaurants, museums, cafes, and tourist attractions.
        /// </summary>
        Task<List<FetchedPlaceDto>> FetchNearbyPlacesAsync(
            double latitude,
            double longitude,
            double minRadiusKm = 4,
            double maxRadiusKm = 20);

        /// <summary>
        /// Sends the list of fetched places and user preferences to Gemini AI
        /// and returns a rich, structured JSON trip plan.
        /// </summary>
        Task<string> GenerateTripPlanAsync(
            List<FetchedPlaceDto> places,
            int durationDays = 3,
            string? budget = null,
            string? location = null,
            List<TourismType>? tourismTypes = null,
            string? preferences = null);
    }
}
