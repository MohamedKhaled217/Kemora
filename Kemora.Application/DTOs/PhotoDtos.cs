using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    public class CreatePhotoDto
    {
        [Required(ErrorMessage = "Photo URL is required.")]
        [Url(ErrorMessage = "Photo URL must be a valid URL.")]
        public string ImageURL { get; set; } = string.Empty;

        public bool IsMain { get; set; }
    }

    public class PhotoResponseDto
    {
        public int PhotoID { get; set; }
        public string ImageURL { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int PlaceID { get; set; }
    }
}
