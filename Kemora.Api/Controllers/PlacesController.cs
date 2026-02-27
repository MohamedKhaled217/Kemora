using Kemora.Api.DTOs;
using Kemora.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kemora.Api.Controllers
{
    /// <summary>
    /// Provides endpoints to discover nearby places via Google Maps
    /// and generate AI-powered trip plans using a local language model.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        private readonly ILogger<PlacesController> _logger;

        public PlacesController(IPlaceService placeService, ILogger<PlacesController> logger)
        {
            _placeService = placeService;
            _logger = logger;
        }

        // ──────────────────────────────────────────────────────────────────────
        // POST /api/places/nearby
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Fetch hotels, restaurants, museums, cafes, and tourist attractions
        /// within a radius band [minRadiusKm, maxRadiusKm] around the supplied coordinates.
        /// </summary>
        /// <remarks>
        /// Example request body:
        /// <code>
        /// { "latitude": 30.0444, "longitude": 31.2357, "minRadiusKm": 4, "maxRadiusKm": 15 }
        /// </code>
        /// </remarks>
        [HttpPost("nearby")]
        [ProducesResponseType(typeof(NearbyPlacesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NearbyPlacesResponseDto>> GetNearbyPlaces(
            [FromBody] NearbyPlacesRequestDto request)
        {
            if (request.MinRadiusKm >= request.MaxRadiusKm)
                return BadRequest("MinRadiusKm must be less than MaxRadiusKm.");

            try
            {
                _logger.LogInformation(
                    "Fetching places near ({Lat}, {Lng}) within {Min}–{Max} km",
                    request.Latitude, request.Longitude, request.MinRadiusKm, request.MaxRadiusKm);

                var places = await _placeService.FetchNearbyPlacesAsync(
                    request.Latitude, request.Longitude,
                    request.MinRadiusKm, request.MaxRadiusKm);

                return Ok(new NearbyPlacesResponseDto
                {
                    TotalCount = places.Count,
                    Places     = places
                });
            }
            catch (InvalidOperationException ex)
            {
                // Missing API key or config
                _logger.LogError(ex, "Configuration error in GetNearbyPlaces");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetNearbyPlaces");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred while fetching places." });
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // POST /api/places/trip-plan
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Fetches nearby places and sends them to the local AI model to generate
        /// a complete, day-by-day trip itinerary.
        /// </summary>
        /// <remarks>
        /// Requires a Gemini API key configured in appsettings (Gemini:ApiKey).
        /// Example request body:
        /// <code>
        /// {
        ///   "latitude": 30.0444,
        ///   "longitude": 31.2357,
        ///   "minRadiusKm": 0,
        ///   "maxRadiusKm": 5,
        ///   "durationDays": 3,
        ///   "budget": "Mid-Range",
        ///   "location": "Cairo",
        ///   "tourismTypes": ["CulturalHeritage", "Culinary"],
        ///   "preferences": "solo traveler, loves history"
        /// }
        /// </code>
        /// </remarks>
        [HttpPost("trip-plan")]
        [ProducesResponseType(typeof(TripPlanResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TripPlanResponseDto>> GenerateTripPlan(
            [FromBody] TripPlanRequestDto request)
        {
            if (request.MinRadiusKm >= request.MaxRadiusKm)
                return BadRequest("MinRadiusKm must be less than MaxRadiusKm.");

            try
            {
                _logger.LogInformation(
                    "Generating {Days}-day trip plan near ({Lat}, {Lng}), location: {Loc}, budget: {Budget}",
                    request.DurationDays, request.Latitude, request.Longitude,
                    request.Location ?? "not specified", request.Budget ?? "Mid-Range");

                // Step 1 — fetch places from Overpass
                var places = await _placeService.FetchNearbyPlacesAsync(
                    request.Latitude, request.Longitude,
                    request.MinRadiusKm, request.MaxRadiusKm);

                if (places.Count == 0)
                {
                    return Ok(new TripPlanResponseDto
                    {
                        TotalPlacesFound = 0,
                        Places           = [],
                        TripPlan         = "No places found in the specified area. Try enlarging the radius."
                    });
                }

                // Step 2 — generate trip plan via Gemini
                var tripPlan = await _placeService.GenerateTripPlanAsync(
                    places,
                    request.DurationDays,
                    request.Budget,
                    request.Location,
                    request.TourismTypes,
                    request.Preferences);

                return Ok(new TripPlanResponseDto
                {
                    TotalPlacesFound = places.Count,
                    Places           = places,
                    TripPlan         = tripPlan
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Configuration error in GenerateTripPlan");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GenerateTripPlan");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred while generating the trip plan." });
            }
        }
    }
}
