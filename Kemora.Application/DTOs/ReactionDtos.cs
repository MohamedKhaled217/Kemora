using System.ComponentModel.DataAnnotations;

namespace Kemora.Application.DTOs
{
    public class CreateReactionDto
    {
        [Required, RegularExpression("^(Like|Love|Wow|Sad|Angry)$",
            ErrorMessage = "ReactionType must be one of: Like, Love, Wow, Sad, Angry.")]
        public string ReactionType { get; set; } = "Like";
    }

    public class ReactionResponseDto
    {
        public string UserID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ReactionType { get; set; } = string.Empty;
        public DateTime ReactedAt { get; set; }
    }

    public class ReactionSummaryDto
    {
        public int TotalCount { get; set; }
        public Dictionary<string, int> ByType { get; set; } = [];
    }
}
