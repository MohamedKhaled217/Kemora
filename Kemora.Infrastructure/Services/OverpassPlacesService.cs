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
    /// <summary>
    /// Fetches places from the OpenStreetMap Overpass API (completely free, no key needed)
    /// and generates AI trip plans via Google Gemini.
    /// </summary>
    public class OverpassPlacesService : IPlaceService
    {
        private const string OverpassEndpoint = "https://overpass.kumi.systems/api/interpreter";

        // ── OSM tag groups we care about ──────────────────────────────────────
        private static readonly (string Filter, string Label)[] TagGroups =
        [
            ("[\"tourism\"=\"hotel\"]",       "Hotel"),
            ("[\"amenity\"=\"restaurant\"]",  "Restaurant"),
            ("[\"tourism\"=\"museum\"]",      "Museum"),
            ("[\"amenity\"=\"cafe\"]",        "Cafe"),
            ("[\"tourism\"=\"attraction\"]",  "Tourist Attraction"),
            ("[\"tourism\"=\"viewpoint\"]",   "Viewpoint"),
            ("[\"historic\"]",               "Historical Site"),
        ];

        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<OverpassPlacesService> _logger;

        public OverpassPlacesService(
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ILogger<OverpassPlacesService> logger)
        {
            _httpFactory = httpFactory;
            _config = config;
            _logger = logger;
        }

        // ---------------------------------------------------------------
        // 1. FETCH NEARBY PLACES
        // ---------------------------------------------------------------
        public async Task<List<FetchedPlaceDto>> FetchNearbyPlacesAsync(
            double latitude,
            double longitude,
            double minRadiusKm = 4,
            double maxRadiusKm = 20)
        {
            var client = _httpFactory.CreateClient("Overpass");
            var maxRadiusMetres = (int)(maxRadiusKm * 1000);
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lng = longitude.ToString(CultureInfo.InvariantCulture);

            var sb = new StringBuilder();
            sb.AppendLine("[out:json][timeout:60];");
            sb.AppendLine("(");
            foreach (var (filter, _) in TagGroups)
            {
                sb.AppendLine($"  node{filter}(around:{maxRadiusMetres},{lat},{lng});");
                sb.AppendLine($"  way{filter}(around:{maxRadiusMetres},{lat},{lng});");
            }
            sb.AppendLine(");");
            sb.AppendLine("out center body;");

            var query = sb.ToString();
            _logger.LogInformation("Overpass query:\n{Query}", query);

            try
            {
                var formContent = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("data", query)
                ]);

                var response = await client.PostAsync(OverpassEndpoint, formContent);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Overpass API error: {Status} — {Body}",
                        response.StatusCode, body);
                    return [];
                }

                var result = JsonSerializer.Deserialize<OverpassResponse>(body);
                if (result?.Elements == null || result.Elements.Count == 0)
                {
                    _logger.LogInformation("Overpass returned 0 elements.");
                    return [];
                }

                var places = new List<FetchedPlaceDto>();

                foreach (var el in result.Elements)
                {
                    double elLat = el.Lat ?? el.Center?.Lat ?? 0;
                    double elLng = el.Lon ?? el.Center?.Lon ?? 0;
                    if (elLat == 0 && elLng == 0) continue;

                    var distKm = HaversineKm(latitude, longitude, elLat, elLng);
                    if (distKm < minRadiusKm) continue;

                    var tags = el.Tags ?? new Dictionary<string, string>();

                    var name = tags.GetValueOrDefault("name:en")
                            ?? tags.GetValueOrDefault("name");

                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var types = InferTypes(tags);

                    tags.TryGetValue("phone", out var phone);
                    tags.TryGetValue("website", out var website);
                    tags.TryGetValue("addr:full", out var addrFull);
                    tags.TryGetValue("addr:street", out var addrStreet);
                    tags.TryGetValue("addr:city", out var addrCity);

                    var address = addrFull
                        ?? string.Join(", ",
                            new[] { addrStreet, addrCity }.Where(s => !string.IsNullOrEmpty(s)));

                    tags.TryGetValue("opening_hours", out var hours);

                    tags.TryGetValue("stars", out var starsStr);
                    double? rating = double.TryParse(starsStr, CultureInfo.InvariantCulture, out var s)
                        ? s : null;

                    places.Add(new FetchedPlaceDto
                    {
                        Name         = name,
                        Address      = address ?? string.Empty,
                        Types        = types,
                        Rating       = rating,
                        PriceLevel   = null,
                        Latitude     = elLat,
                        Longitude    = elLng,
                        Phone        = phone,
                        Website      = website,
                        OpeningHours = string.IsNullOrEmpty(hours) ? null : [hours],
                        DistanceKm   = Math.Round(distKm, 2)
                    });
                }

                return places
                    .GroupBy(p => $"{p.Name}|{p.Latitude:F5}|{p.Longitude:F5}")
                    .Select(g => g.First())
                    .OrderBy(p => p.DistanceKm)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to query Overpass API");
                return [];
            }
        }

        // ---------------------------------------------------------------
        // 2. GENERATE TRIP PLAN (Google Gemini API)
        // ---------------------------------------------------------------
        public async Task<string> GenerateTripPlanAsync(
            List<FetchedPlaceDto> places,
            int durationDays = 3,
            string? budget = null,
            string? location = null,
            List<TourismType>? tourismTypes = null,
            string? preferences = null)
        {
            var apiKey = _config["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Gemini:ApiKey is not configured in appsettings.");
            var model  = _config["Gemini:Model"] ?? "gemini-2.0-flash";

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            // ── Build the rich prompt ────────────────────────────────────
            var radiusMeters = places.Count > 0
                ? (int)(places.Max(p => p.DistanceKm) * 1000)
                : 5000;

            var locationName = location ?? "Egypt";
            var tourismInterestsStr = tourismTypes?.Count > 0
                ? string.Join(", ", tourismTypes.Select(t => t switch
                {
                    TourismType.Leisure            => "Leisure & Relaxation",
                    TourismType.CulturalHeritage   => "Cultural & Heritage sites",
                    TourismType.Adventure          => "Adventure activities",
                    TourismType.EcoTourism         => "Eco-Tourism & Nature",
                    TourismType.Business           => "Business & MICE",
                    TourismType.MedicalWellness    => "Medical & Wellness",
                    TourismType.ReligiousPilgrimage => "Religious & Pilgrimage",
                    TourismType.Sports             => "Sports & Outdoor",
                    TourismType.Culinary           => "Culinary & Food tours",
                    _                              => t.ToString()
                }))
                : "General sightseeing";

            var budgetTier = budget ?? "Mid-Range";
            var travelerInfo = preferences ?? "solo traveler";

            // Limit to top 140 places to avoid exceeding Gemini token limits (429 TooManyRequests) but allow variety
            var topPlaces = places
                .OrderByDescending(p => p.Rating ?? 0)
                .ThenBy(p => p.DistanceKm)
                .Take(140)
                .ToList();

            var placeSummary = BuildPlaceSummary(topPlaces);

            var tourismTypesList = tourismTypes ?? [];
            var tourismTypesJson = string.Join(", ", tourismTypesList.Select(t => $"\"{t}\""));

var prompt = $$"""
You are a professional travel consultant and certified tour operator specializing in {{locationName}}, with deep expertise in local culture, history, and culinary tourism. Generate a premium {{durationDays}}-day itinerary for a {{travelerInfo}} passionate about {{tourismInterestsStr}}.

## CONSTRAINTS & REQUIREMENTS:
- Generate strictly between 3 and 6 activities per day.
- Do NOT recommend the same place, activity, or restaurant more than once across the entire itinerary. Every suggestion must be unique.
- All activities within each day must be located within a {{radiusMeters}}-meter radius of each other to minimize transit time.
- Group activities geographically: morning/afternoon/evening slots should follow a logical route.
- Include a balanced mix of iconic landmarks, hidden gems, and immersive cultural experiences.
- All prices must be current estimates in EGP and USD.
- Provide GPS coordinates (latitude & longitude) for every location.
- Budget tier: {{budgetTier}}.
- ONLY select places from the list provided below. Do NOT invent places not in the list.

## AVAILABLE PLACES TO CHOOSE FROM:
{{placeSummary}}

## RESPONSE FORMAT:
Respond ONLY with valid, parseable JSON — no markdown, no commentary.

{
  "trip_title": "string — a catchy title",
  "trip_duration": "{{durationDays}} days",
  "traveler_type": "{{travelerInfo}}",
  "daily_budget_estimate": {
    "budget_egp": "string",
    "budget_usd": "string"
  },
  "itinerary": [
    {
      "day": 1,
      "date": "Day 1",
      "activities": [
        {
          "date": "Day 1",
          "time_slot": "morning | afternoon | evening",
          "suggested_hours": "08:00 AM – 11:00 AM",
          "place": "string — full official place name from the list above",
          "latitude": number,
          "longitude": number,
          "category": "landmark | museum | dining",
          "description": "string — what to see and do",
          "price": "string — entry fee or expected cost in EGP/USD",
          "time_needed": "string",
          "food_recommendation": {
            "place": "string — a nearby restaurant from the list",
            "price": "string — approximate meal cost"
          }
        }
      ]
    }
  ],
  "general_tips": [
    "string — 3-5 useful tips"
  ]
}
""";

            _logger.LogInformation(
                "Generating {Days}-day Gemini trip plan for {Location}, interests: {Interests}, budget: {Budget}",
                durationDays, locationName, tourismInterestsStr, budgetTier);

            // ── Call Gemini REST API ──────────────────────────────────────
            var client = _httpFactory.CreateClient("LocalAI");

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.2,
                    maxOutputTokens = 8192,
                    responseMimeType = "application/json"
                }
            };

            try
            {
                var response = await client.PostAsJsonAsync(endpoint, requestBody);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Gemini API error: {Status} — {Body}",
                        response.StatusCode, responseBody);
                    return $"Gemini API error ({response.StatusCode}): {responseBody}";
                }

                // Parse the Gemini response envelope
                using var doc = JsonDocument.Parse(responseBody);
                var candidates = doc.RootElement.GetProperty("candidates");
                var text = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "No response from Gemini.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to contact Gemini API at {Endpoint}", endpoint);
                return $"Error contacting Gemini API: {ex.Message}";
            }
        }

        // ---------------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------------

        /// <summary>Haversine formula: great-circle distance in km.</summary>
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

        /// <summary>Maps OSM tags to human-readable place type labels.</summary>
        private static List<string> InferTypes(Dictionary<string, string> tags)
        {
            var types = new List<string>();

            if (tags.TryGetValue("tourism", out var tourism))
            {
                types.Add(tourism switch
                {
                    "hotel"       => "Hotel",
                    "museum"      => "Museum",
                    "attraction"  => "Tourist Attraction",
                    "viewpoint"   => "Viewpoint",
                    _             => tourism
                });
            }

            if (tags.TryGetValue("amenity", out var amenity))
            {
                types.Add(amenity switch
                {
                    "restaurant" => "Restaurant",
                    "cafe"       => "Cafe",
                    _            => amenity
                });
            }

            if (tags.ContainsKey("historic"))
            {
                types.Add("Historical Site");
            }

            return types.Count > 0 ? types : ["Place"];
        }

        private static string BuildPlaceSummary(List<FetchedPlaceDto> places)
        {
            var sb = new StringBuilder();
            int i = 1;
            foreach (var p in places)
            {
                var types  = string.Join(", ", p.Types);
                var rating = p.Rating.HasValue ? $"{p.Rating:F1}/5" : "no rating";
                sb.AppendLine($"{i++}. {p.Name} ({types}) — {rating}");
                sb.AppendLine($"   Lat: {p.Latitude}, Lon: {p.Longitude}");
                sb.AppendLine($"   Address : {p.Address}");
                sb.AppendLine($"   Distance: {p.DistanceKm} km from center");
                if (!string.IsNullOrEmpty(p.Website))
                    sb.AppendLine($"   Website : {p.Website}");
                if (p.OpeningHours?.Count > 0)
                    sb.AppendLine($"   Hours   : {string.Join(" | ", p.OpeningHours)}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        // ---------------------------------------------------------------
        // PRIVATE RESPONSE MODELS (Overpass API)
        // ---------------------------------------------------------------

        private sealed class OverpassResponse
        {
            [JsonPropertyName("elements")]
            public List<OverpassElement>? Elements { get; set; }
        }

        private sealed class OverpassElement
        {
            [JsonPropertyName("type")]
            public string? Type { get; set; }

            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("lat")]
            public double? Lat { get; set; }

            [JsonPropertyName("lon")]
            public double? Lon { get; set; }

            [JsonPropertyName("center")]
            public OverpassCenter? Center { get; set; }

            [JsonPropertyName("tags")]
            public Dictionary<string, string>? Tags { get; set; }
        }

        private sealed class OverpassCenter
        {
            [JsonPropertyName("lat")]
            public double Lat { get; set; }

            [JsonPropertyName("lon")]
            public double Lon { get; set; }
        }
    }
}
