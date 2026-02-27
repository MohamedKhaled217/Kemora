namespace Kemora.Application.DTOs
{
    public class FavoriteResponseDto
    {
        public int PlaceID { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public string PlaceAddress { get; set; } = string.Empty;
        public string? MainImageURL { get; set; }
    }

    public class FavoriteCheckDto
    {
        public bool IsFavorited { get; set; }
    }
}
