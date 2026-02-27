using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kemora.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("posts/{postId}/comments")]
        public async Task<ActionResult<CommentResponseDto>> CreateComment(
            int postId, [FromBody] CreateCommentDto dto)
        {
            if (!await _context.Posts.AnyAsync(p => p.PostID == postId))
                return NotFound("Post not found.");

            var userId = GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            var comment = new Comment
            {
                Content   = dto.Content,
                CreatedAt = DateTime.UtcNow,
                PostID    = postId,
                UserID    = userId,
                Media     = dto.Media?.Select(m => new CommentMedia
                {
                    MediaURL  = m.MediaURL,
                    MediaType = m.MediaType
                }).ToList() ?? []
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComments), new { postId },
                new CommentResponseDto
                {
                    CommentID     = comment.CommentID,
                    Content       = comment.Content,
                    CreatedAt     = comment.CreatedAt,
                    AuthorId      = userId,
                    AuthorName    = user!.FullName,
                    Media         = comment.Media.Select(m => new CommentMediaResponseDto
                        { MediaID = m.MediaID, MediaURL = m.MediaURL, MediaType = m.MediaType }).ToList(),
                    ReactionCount = 0
                });
        }

        [HttpGet("posts/{postId}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CommentResponseDto>>> GetComments(
            int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (!await _context.Posts.AnyAsync(p => p.PostID == postId))
                return NotFound("Post not found.");

            return Ok(await _context.Comments
                .Include(c => c.User).Include(c => c.Media).Include(c => c.Reactions)
                .Where(c => c.PostID == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(c => new CommentResponseDto
                {
                    CommentID     = c.CommentID,
                    Content       = c.Content,
                    CreatedAt     = c.CreatedAt,
                    AuthorId      = c.UserID,
                    AuthorName    = c.User.FullName,
                    Media         = c.Media.Select(m => new CommentMediaResponseDto
                        { MediaID = m.MediaID, MediaURL = m.MediaURL, MediaType = m.MediaType }).ToList(),
                    ReactionCount = c.Reactions.Count
                }).ToListAsync());
        }

        [HttpPut("comments/{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto dto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();
            if (comment.UserID != GetUserId()) return Forbid();
            comment.Content = dto.Content;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();
            if (comment.UserID != GetUserId()) return Forbid();
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
