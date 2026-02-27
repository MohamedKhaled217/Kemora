using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kemora.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ReactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReactionsController(ApplicationDbContext context) => _context = context;

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ═══════════════════════════════════════════════════════════════════════
        // POST REACTIONS
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>React to a post (upsert — changes type if already reacted).</summary>
        [HttpPost("posts/{postId}/reactions")]
        public async Task<IActionResult> ReactToPost(int postId, [FromBody] CreateReactionDto dto)
        {
            if (!await _context.Posts.AnyAsync(p => p.PostID == postId))
                return NotFound("Post not found.");

            var userId = GetUserId();
            var existing = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostID == postId && r.UserID == userId);

            if (existing != null)
            {
                existing.ReactionType = dto.ReactionType;
                existing.ReactedAt = DateTime.UtcNow;
            }
            else
            {
                _context.PostReactions.Add(new PostReaction
                {
                    PostID       = postId,
                    UserID       = userId,
                    ReactionType = dto.ReactionType,
                    ReactedAt    = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Reaction saved." });
        }

        /// <summary>Remove own reaction from a post.</summary>
        [HttpDelete("posts/{postId}/reactions")]
        public async Task<IActionResult> UnreactFromPost(int postId)
        {
            var userId = GetUserId();
            var reaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostID == postId && r.UserID == userId);
            if (reaction == null) return NotFound("You haven't reacted to this post.");

            _context.PostReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Get reaction summary for a post.</summary>
        [HttpGet("posts/{postId}/reactions")]
        [AllowAnonymous]
        public async Task<ActionResult<ReactionSummaryDto>> GetPostReactions(int postId)
        {
            if (!await _context.Posts.AnyAsync(p => p.PostID == postId))
                return NotFound("Post not found.");

            var reactions = await _context.PostReactions
                .Where(r => r.PostID == postId).ToListAsync();

            return Ok(new ReactionSummaryDto
            {
                TotalCount = reactions.Count,
                ByType     = reactions.GroupBy(r => r.ReactionType)
                                      .ToDictionary(g => g.Key, g => g.Count())
            });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // COMMENT REACTIONS
        // ═══════════════════════════════════════════════════════════════════════

        [HttpPost("comments/{commentId}/reactions")]
        public async Task<IActionResult> ReactToComment(int commentId, [FromBody] CreateReactionDto dto)
        {
            if (!await _context.Comments.AnyAsync(c => c.CommentID == commentId))
                return NotFound("Comment not found.");

            var userId = GetUserId();
            var existing = await _context.Set<CommentReaction>()
                .FirstOrDefaultAsync(r => r.CommentID == commentId && r.UserID == userId);

            if (existing != null)
            {
                existing.ReactionType = dto.ReactionType;
                existing.ReactedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Set<CommentReaction>().Add(new CommentReaction
                {
                    CommentID    = commentId,
                    UserID       = userId,
                    ReactionType = dto.ReactionType,
                    ReactedAt    = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Reaction saved." });
        }

        [HttpDelete("comments/{commentId}/reactions")]
        public async Task<IActionResult> UnreactFromComment(int commentId)
        {
            var userId = GetUserId();
            var reaction = await _context.Set<CommentReaction>()
                .FirstOrDefaultAsync(r => r.CommentID == commentId && r.UserID == userId);
            if (reaction == null) return NotFound("You haven't reacted to this comment.");

            _context.Set<CommentReaction>().Remove(reaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
