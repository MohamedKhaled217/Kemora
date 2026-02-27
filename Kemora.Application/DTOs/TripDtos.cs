using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    public class CreateTripDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
    }

    public class UpdateTripDto
    {
        [StringLength(200)] public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class TripListDto
    {
        public int TripID { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PlaceCount { get; set; }
    }

    public class TripDetailDto
    {
        public int TripID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TripPlaceResponseDto> Places { get; set; } = [];
    }

    public class AddTripPlaceDto
    {
        [Required] public int PlaceID { get; set; }
        [Required] public DateTime VisitDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateTripPlaceDto
    {
        public DateTime? VisitDate { get; set; }
        public string? Notes { get; set; }
    }

    public class TripPlaceResponseDto
    {
        public int TripPlaceID { get; set; }
        public int PlaceID { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string? Notes { get; set; }
    }
}
