using System.ComponentModel.DataAnnotations;

namespace Kemora.Api.DTOs
{
    public class CreateReviewDto
    {
        [Required, Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required, StringLength(2000)]
        public string Text { get; set; } = string.Empty;
    }

    public class ReviewResponseDto
    {
        public int ReviewID { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
        public int PlaceID { get; set; }
    }
}
