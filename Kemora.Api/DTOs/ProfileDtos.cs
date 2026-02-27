using System.ComponentModel.DataAnnotations;

namespace Kemora.Api.DTOs
{
    public class UpdateProfileDto
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;
    }

    public class ProfileResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int PostCount { get; set; }
        public int TripCount { get; set; }
        public int BadgeCount { get; set; }
        public int FavoriteCount { get; set; }
    }

    public class PublicProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int BadgeCount { get; set; }
        public int PostCount { get; set; }
    }
}
