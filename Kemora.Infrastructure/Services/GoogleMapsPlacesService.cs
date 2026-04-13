using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kemora.Domain.Enums;
using Kemora.Domain.Interfaces;
using Kemora.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kemora.Infrastructure.Services
{
    public class GoogleMapsPlacesService : IPlaceService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<GoogleMapsPlacesService> _logger;

        public GoogleMapsPlacesService(
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ILogger<GoogleMapsPlacesService> logger)
        {
            _httpFactory = httpFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<List<FetchedPlaceDto>> FetchNearbyPlacesAsync(
            double latitude,
            double longitude,
            double minRadiusKm = 4,
            double maxRadiusKm = 20)
        {
            var apiKey = _config["GoogleMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("GoogleMaps:ApiKey is missing.");
                return [];
            }

            var client = _httpFactory.CreateClient("GoogleMaps");
            var radiusMeters = (int)(maxRadiusKm * 1000);
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lng = longitude.ToString(CultureInfo.InvariantCulture);

            // Fetch a wide range of categories for a balanced and dense tourism profile
            var types = new[] { "tourist_attraction", "museum", "restaurant", "cafe", "lodging", "point_of_interest", "park", "art_gallery", "natural_feature" };
            var allResults = new List<GooglePlaceResult>();

            // PASS 1: Broad Category Search
            foreach (var type in types)
            {
                var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lng}&radius={radiusMeters}&type={type}&key={apiKey}";
                await FetchAndAddResultsWithPagination(url, client, type, allResults, apiKey, 2); // Fetch up to 2 pages (40 results)
            }

            // PASS 2: Famous Landmarks Keyword Search (Drastic Density Improvement)
            var keywords = new[] { "famous landmarks", "top attractions", "historic sites" };
            foreach (var kw in keywords)
            {
                var kwUrl = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(kw)}+in+{lat},{lng}&radius={radiusMeters}&key={apiKey}";
                await FetchAndAddResultsWithPagination(kwUrl, client, $"Keyword:{kw}", allResults, apiKey, 2); 
            }

            var places = new List<FetchedPlaceDto>();
            foreach (var res in allResults.DistinctBy(r => r.PlaceId))
            {
                var elLat = res.Geometry?.Location?.Lat ?? 0;
                var elLng = res.Geometry?.Location?.Lng ?? 0;
                if (elLat == 0 && elLng == 0) continue;

                var distKm = HaversineKm(latitude, longitude, elLat, elLng);
                if (distKm < minRadiusKm) continue;

                var imageUrl = "";
                if (res.Photos != null && res.Photos.Count > 0)
                {
                    imageUrl = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=800&photo_reference={res.Photos[0].PhotoReference}&key={apiKey}";
                }

                places.Add(new FetchedPlaceDto
                {
                    Name = res.Name ?? "Unknown Place",
                    Address = res.Vicinity ?? string.Empty,
                    Types = res.Types ?? ["place"],
                    Rating = res.Rating,
                    PriceLevel = res.PriceLevel?.ToString(),
                    Latitude = elLat,
                    Longitude = elLng,
                    DistanceKm = Math.Round(distKm, 2),
                    ImageUrl = imageUrl,
                    GooglePlaceId = res.PlaceId
                });
            }

            return places.OrderBy(p => p.DistanceKm).ToList();
        }

        public async Task<FetchedPlaceDto?> GetPlaceDetailsAsync(string googlePlaceId)
        {
            var apiKey = _config["GoogleMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey)) return null;

            var client = _httpFactory.CreateClient("GoogleMaps");
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={googlePlaceId}&fields=name,rating,formatted_address,geometry,photo,type,formatted_phone_number,website,price_level&key={apiKey}";

            try
            {
                var response = await client.GetFromJsonAsync<GooglePlaceDetailsResponse>(url);
                if (response?.Result == null) return null;

                var res = response.Result;
                var imageUrl = "";
                if (res.Photos != null && res.Photos.Count > 0)
                {
                    imageUrl = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=800&photo_reference={res.Photos[0].PhotoReference}&key={apiKey}";
                }

                return new FetchedPlaceDto
                {
                    GooglePlaceId = googlePlaceId,
                    Name = res.Name ?? "Unknown",
                    Address = res.FormattedAddress ?? (res.Vicinity ?? ""),
                    Rating = res.Rating,
                    PriceLevel = res.PriceLevel?.ToString(),
                    Latitude = res.Geometry?.Location?.Lat ?? 0,
                    Longitude = res.Geometry?.Location?.Lng ?? 0,
                    Phone = res.FormattedPhoneNumber,
                    Website = res.Website,
                    Types = res.Types ?? [],
                    ImageUrl = imageUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch place details for {PlaceId}", googlePlaceId);
                return null;
            }
        }

        private async Task FetchAndAddResultsWithPagination(string baseUrl, HttpClient client, string logLabel, List<GooglePlaceResult> outputList, string apiKey, int maxPages)
        {
            var currentUrl = baseUrl;
            for (int i = 0; i < maxPages; i++)
            {
                try
                {
                    var response = await client.GetFromJsonAsync<GoogleNearbySearchResponse>(currentUrl);
                    if (response?.Results != null)
                    {
                        _logger.LogInformation("Google Maps {Label} (Page {Page}) found {Count} results.", logLabel, i + 1, response.Results.Count);
                        outputList.AddRange(response.Results);
                    }

                    if (string.IsNullOrEmpty(response?.NextPageToken)) break;

                    // Next page requires a 2-second delay according to Google API guidelines
                    await Task.Delay(2000);
                    currentUrl = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?pagetoken={response.NextPageToken}&key={apiKey}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed Google Maps call for {Label} on Page {Page}", logLabel, i + 1);
                    break;
                }
            }
        }

        public async Task<string> GenerateTripPlanAsync(
            List<FetchedPlaceDto> places,
            int durationDays = 3,
            string? budget = null,
            string? location = null,
            List<TourismType>? tourismTypes = null,
            string? preferences = null)
        {
            var apiKey = _config["WisdomGate:ApiKey"];
            var model = _config["WisdomGate:Model"] ?? "default";
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("WisdomGate:ApiKey is missing.");
                return "AI Error: Mission API Key.";
            }

            var endpoint = "chat/completions";

            var locationName = location ?? "Egypt";
            var tourismInterestsStr = tourismTypes?.Count > 0
                ? string.Join(", ", tourismTypes.Select(t => t.ToString()))
                : "General sightseeing";

            var budgetTier = budget ?? "Mid-Range";
            var travelerInfo = preferences ?? "traveler";

            var placeSummary = BuildCompactPlaceSummary(places.Take(30).ToList());

            var prompt = $@"
You are a premium, certified luxury travel consultant specializing in {locationName}. 
Generate a comprehensive {durationDays}-day itinerary for a {travelerInfo} interested in {tourismInterestsStr}.

GOAL: 
Provide a high-density, high-value journey that guides the traveler deeply through each day.

CONSTRAINTS:
- DENSITY: Strictly 4 to 6 activities per day.
- QUALITY: Descriptions must be 2-3 engaging, informative sentences.
- LOGIC: Activities must follow a logical geographical route (minimizing travel time).
- CONTENT: Include breakfast/lunch/dinner recommendations and local hidden gems.
- METADATA: Provide a summary of the day's vibe and specific transport guidance (e.g., 'Take an Uber', 'Walkable district').
- BUDGET: {budgetTier}.
- SOURCE: ONLY select from the list below.

AVAILABLE PLACES:
{placeSummary}

Respond ONLY with valid JSON (application/json):
{{
  ""trip_title"": ""A Captivating {durationDays}-Day Journey through {locationName}"",
  ""trip_duration"": ""{durationDays} days"",
  ""itinerary"": [
    {{
      ""day"": 1,
      ""daily_summary"": ""A brief 2-sentence overview of what makes this day special."",
      ""transport_tips"": ""Expert advice on how to get around today efficiently."",
      ""activities"": [
        {{
          ""time_slot"": ""morning | afternoon | evening"",
          ""suggested_hours"": ""HH:MM AM - HH:MM PM"",
          ""place"": ""Full official name from list"",
          ""image_url"": ""The ImageURL provided in the AVAILABLE PLACES list below"",
          ""latitude"": number,
          ""longitude"": number,
          ""description"": ""2-3 sentences of rich, expert context."",
          ""rating"": number,
          ""price"": ""Estimated EGP/USD""
        }}
      ]
    }}
  ]
}}";

            var client = _httpFactory.CreateClient("LocalAI");
            var payload = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful travel planning assistant." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.3,
                response_format = new { type = "json_object" }
            };

            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                var response = await client.PostAsJsonAsync(endpoint, payload);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    _logger.LogError("WisdomGate API error: {Error}", err);
                    return $"AI Error: {response.StatusCode}";
                }

                var result = await response.Content.ReadFromJsonAsync<WisdomGateResponse>();
                return result?.Choices?[0]?.Message?.Content ?? "AI returned empty response.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call WisdomGate API");
                return $"AI Error: {ex.Message}";
            }
        }

        public async Task<string> SwapPlaceAsync(string currentPlaceName, string preferences)
        {
            // Simplified swap logic using WisdomGate
            var apiKey = _config["WisdomGate:ApiKey"];
            var model = _config["WisdomGate:Model"] ?? "default";
            var endpoint = "chat/completions";

            var prompt = $"Suggest ONE alternative to '{currentPlaceName}' for a traveler with preferences: '{preferences}'. Respond with JSON: {{ \"place\": \"name\", \"description\": \"brief reason\", \"latitude\": 0, \"longitude\": 0, \"price\": \"$\" }}";

            var client = _httpFactory.CreateClient("LocalAI");
            var payload = new
            {
                model = model,
                messages = new[] { new { role = "user", content = prompt } },
                response_format = new { type = "json_object" }
            };

            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                var response = await client.PostAsJsonAsync(endpoint, payload);
                var result = await response.Content.ReadFromJsonAsync<WisdomGateResponse>();
                return result?.Choices?[0]?.Message?.Content ?? "";
            }
            catch { return ""; }
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

        private static string BuildCompactPlaceSummary(List<FetchedPlaceDto> places)
        {
            var sb = new StringBuilder();
            foreach (var p in places)
            {
                var rating = p.Rating.HasValue ? $"{p.Rating:F1}★" : "?";
                sb.AppendLine($"- {p.Name} (ImageURL: {p.ImageUrl}) {rating} {p.Latitude:F4},{p.Longitude:F4}");
            }
            return sb.ToString();
        }

        // --- DTOs for JSON deserialization ---
        private class GoogleNearbySearchResponse
        {
            [JsonPropertyName("results")] public List<GooglePlaceResult>? Results { get; set; }
            [JsonPropertyName("next_page_token")] public string? NextPageToken { get; set; }
        }

        private class GooglePlaceResult
        {
            [JsonPropertyName("place_id")] public string? PlaceId { get; set; }
            [JsonPropertyName("name")] public string? Name { get; set; }
            [JsonPropertyName("geometry")] public GoogleGeometry? Geometry { get; set; }
            [JsonPropertyName("rating")] public double? Rating { get; set; }
            [JsonPropertyName("price_level")] public int? PriceLevel { get; set; }
            [JsonPropertyName("types")] public List<string>? Types { get; set; }
            [JsonPropertyName("vicinity")] public string? Vicinity { get; set; }
            [JsonPropertyName("formatted_address")] public string? FormattedAddress { get; set; }
            [JsonPropertyName("formatted_phone_number")] public string? FormattedPhoneNumber { get; set; }
            [JsonPropertyName("website")] public string? Website { get; set; }
            [JsonPropertyName("photos")] public List<GooglePhoto>? Photos { get; set; }
        }

        private class GooglePlaceDetailsResponse
        {
            [JsonPropertyName("result")] public GooglePlaceResult? Result { get; set; }
            [JsonPropertyName("status")] public string? Status { get; set; }
        }

        private class GoogleGeometry { [JsonPropertyName("location")] public GoogleLocation? Location { get; set; } }
        private class GoogleLocation { [JsonPropertyName("lat")] public double Lat { get; set; } [JsonPropertyName("lng")] public double Lng { get; set; } }
        private class GooglePhoto { [JsonPropertyName("photo_reference")] public string? PhotoReference { get; set; } }

        private class WisdomGateResponse
        {
            [JsonPropertyName("choices")] public List<WisdomGateChoice>? Choices { get; set; }
        }
        private class WisdomGateChoice { [JsonPropertyName("message")] public WisdomGateMessage? Message { get; set; } }
        private class WisdomGateMessage { [JsonPropertyName("content")] public string? Content { get; set; } }
    }
}
