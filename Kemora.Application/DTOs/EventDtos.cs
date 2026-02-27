using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    public class CreateEventDto
    {
        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(200, ErrorMessage = "Event name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }
    }

    public class UpdateEventDto
    {
        [StringLength(200)] public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class EventResponseDto
    {
        public int EventID { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PlaceID { get; set; }
        public string PlaceName { get; set; } = string.Empty;
    }
}
