using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kemora.Api.Controllers
{
    /// <summary>
    /// React to posts and comments with emoji reactions (like, love, etc.).
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [Authorize]
    public class ReactionsController : ControllerBase
    {
        private readonly IReactionService _reactionService;

        public ReactionsController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        /// <summary>
        /// Add or remove a reaction on a post.
        /// </summary>
        /// <param name="postId">The post to react to.</param>
        /// <param name="dto">Reaction details (action: "add"/"remove", reactionType).</param>
        [HttpPost("posts/{postId}/react")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReactToPost(int postId, [FromBody] ReactToPostDto dto)
        {
            if (await _reactionService.ReactToPostAsync(GetUserId(), postId, dto))
                return NoContent();
            return BadRequest();
        }

        /// <summary>
        /// Add or remove a reaction on a comment.
        /// </summary>
        /// <param name="commentId">The comment to react to.</param>
        /// <param name="dto">Reaction details (action: "add"/"remove", reactionType).</param>
        [HttpPost("comments/{commentId}/react")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReactToComment(int commentId, [FromBody] ReactToCommentDto dto)
        {
            if (await _reactionService.ReactToCommentAsync(GetUserId(), commentId, dto))
                return NoContent();
            return BadRequest();
        }
    }
}
