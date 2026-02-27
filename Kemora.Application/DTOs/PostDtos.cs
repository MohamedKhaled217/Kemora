using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    // ── Request ───────────────────────────────────────────────────────────────

    public class CreatePostDto
    {
        [Required, StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        public List<CreatePostMediaDto>? Media { get; set; }
    }

    public class CreatePostMediaDto
    {
        [Required] public string MediaURL { get; set; } = string.Empty;

        [Required, RegularExpression("^(Image|Video)$", ErrorMessage = "MediaType must be 'Image' or 'Video'.")]
        public string MediaType { get; set; } = "Image";
    }

    public class UpdatePostDto
    {
        [Required, StringLength(5000)]
        public string Content { get; set; } = string.Empty;
    }

    // ── Response ──────────────────────────────────────────────────────────────

    public class PostMediaResponseDto
    {
        public int MediaID { get; set; }
        public string MediaURL { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
    }

    public class PostListResponseDto
    {
        public int PostID { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public List<PostMediaResponseDto> Media { get; set; } = [];
        public int ReactionCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class PostDetailResponseDto : PostListResponseDto
    {
        public List<CommentResponseDto> Comments { get; set; } = [];
    }
}
