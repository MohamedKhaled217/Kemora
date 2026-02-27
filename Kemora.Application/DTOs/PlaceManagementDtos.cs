using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    // ── Governorate ───────────────────────────────────────────────────────────

    public class CreateGovernorateDto
    {
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(100)] public string Region { get; set; } = string.Empty;
    }

    public class GovernorateResponseDto
    {
        public int GovernorateID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public int PlaceCount { get; set; }
    }

    // ── Category ──────────────────────────────────────────────────────────────

    public class CreateCategoryDto
    {
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    }

    public class CategoryResponseDto
    {
        public int CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PlaceTypeResponseDto> PlaceTypes { get; set; } = [];
    }

    // ── PlaceType ─────────────────────────────────────────────────────────────

    public class CreatePlaceTypeDto
    {
        [Required, StringLength(100)] public string GoogleType { get; set; } = string.Empty;
        [Required, StringLength(100)] public string DisplayName { get; set; } = string.Empty;
        [Required] public int CategoryID { get; set; }
    }

    public class PlaceTypeResponseDto
    {
        public int TypeID { get; set; }
        public string GoogleType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    // ── Place (DB-stored) ─────────────────────────────────────────────────────

    public class CreatePlaceDto
    {
        [Required, StringLength(200)] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required, StringLength(500)] public string Address { get; set; } = string.Empty;
        [Required, Range(-90, 90)] public decimal Latitude { get; set; }
        [Required, Range(-180, 180)] public decimal Longitude { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        [Range(0, 5)] public decimal Rating { get; set; }
        [Range(0, 4)] public int PriceLevel { get; set; }
        public string? OpeningHoursJSON { get; set; }
        public string? MainImageURL { get; set; }
        [Required] public int GovernorateID { get; set; }
        [Required] public int PlaceTypeID { get; set; }
    }

    public class UpdatePlaceDto
    {
        [StringLength(200)] public string? Name { get; set; }
        public string? Description { get; set; }
        [StringLength(500)] public string? Address { get; set; }
        [Range(-90, 90)] public decimal? Latitude { get; set; }
        [Range(-180, 180)] public decimal? Longitude { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        [Range(0, 5)] public decimal? Rating { get; set; }
        [Range(0, 4)] public int? PriceLevel { get; set; }
        public string? OpeningHoursJSON { get; set; }
        public string? MainImageURL { get; set; }
        public int? GovernorateID { get; set; }
        public int? PlaceTypeID { get; set; }
    }

    public class PlaceListDto
    {
        public int PlaceID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int PriceLevel { get; set; }
        public string GovernorateName { get; set; } = string.Empty;
        public string PlaceTypeName { get; set; } = string.Empty;
        public string? MainImageURL { get; set; }
    }

    public class PlaceDetailDto
    {
        public int PlaceID { get; set; }
        public string? GooglePlaceID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public decimal Rating { get; set; }
        public int PriceLevel { get; set; }
        public string? OpeningHoursJSON { get; set; }
        public string? MainImageURL { get; set; }
        public string GovernorateName { get; set; } = string.Empty;
        public string PlaceTypeName { get; set; } = string.Empty;
        public List<PhotoResponseDto> Photos { get; set; } = [];
        public List<ReviewResponseDto> Reviews { get; set; } = [];
        public List<EventResponseDto> Events { get; set; } = [];
    }
}
