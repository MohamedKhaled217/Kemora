using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    public class CreateCommentDto
    {
        [Required, StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        public List<CreateCommentMediaDto>? Media { get; set; }
    }

    public class CreateCommentMediaDto
    {
        [Required] public string MediaURL { get; set; } = string.Empty;

        [Required, RegularExpression("^(Image|Video)$")]
        public string MediaType { get; set; } = "Image";
    }

    public class UpdateCommentDto
    {
        [Required, StringLength(2000)]
        public string Content { get; set; } = string.Empty;
    }

    public class CommentMediaResponseDto
    {
        public int MediaID { get; set; }
        public string MediaURL { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
    }

    public class CommentResponseDto
    {
        public int CommentID { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public List<CommentMediaResponseDto> Media { get; set; } = [];
        public int ReactionCount { get; set; }
    }
}
