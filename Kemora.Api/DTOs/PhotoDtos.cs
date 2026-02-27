using System.ComponentModel.DataAnnotations;

namespace Kemora.Api.DTOs
{
    public class CreatePhotoDto
    {
        [Required] public string ImageURL { get; set; } = string.Empty;
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
