namespace Kemora.Domain.Models
{
    /// <summary>
    /// Represents a place returned from the Google Places API (New).
    /// Lives in Domain so Infrastructure and API can share this model without coupling.
    /// </summary>
    public class FetchedPlaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<string> Types { get; set; } = [];
        public double? Rating { get; set; }
        public string? PriceLevel { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public List<string>? OpeningHours { get; set; }
        public double DistanceKm { get; set; }
        public string? ImageUrl { get; set; }
    }
}
