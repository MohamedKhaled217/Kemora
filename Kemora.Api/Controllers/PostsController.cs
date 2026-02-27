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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ──────────────────────────────────────────────────────────────────────
        // CREATE
        // ──────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult<PostListResponseDto>> CreatePost([FromBody] CreatePostDto dto)
        {
            var userId = GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            var post = new Post
            {
                Content   = dto.Content,
                CreatedAt = DateTime.UtcNow,
                UserID    = userId,
                Media     = dto.Media?.Select(m => new PostMedia
                {
                    MediaURL  = m.MediaURL,
                    MediaType = m.MediaType
                }).ToList() ?? []
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.PostID }, MapToListDto(post, user!));
        }

        // ──────────────────────────────────────────────────────────────────────
        // LIST ALL (paginated, newest first)
        // ──────────────────────────────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<PostListResponseDto>>> GetPosts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return Ok(await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Media)
                .Include(p => p.Reactions)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => new PostListResponseDto
                {
                    PostID        = p.PostID,
                    Content       = p.Content,
                    CreatedAt     = p.CreatedAt,
                    AuthorId      = p.UserID,
                    AuthorName    = p.User.FullName,
                    Media         = p.Media.Select(m => new PostMediaResponseDto
                        { MediaID = m.MediaID, MediaURL = m.MediaURL, MediaType = m.MediaType }).ToList(),
                    ReactionCount = p.Reactions.Count,
                    CommentCount  = p.Comments.Count
                }).ToListAsync());
        }

        // ──────────────────────────────────────────────────────────────────────
        // GET SINGLE (detail with comments)
        // ──────────────────────────────────────────────────────────────────────
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostDetailResponseDto>> GetPost(int id)
        {
            var p = await _context.Posts
                .Include(x => x.User)
                .Include(x => x.Media)
                .Include(x => x.Reactions)
                .Include(x => x.Comments).ThenInclude(c => c.User)
                .Include(x => x.Comments).ThenInclude(c => c.Media)
                .Include(x => x.Comments).ThenInclude(c => c.Reactions)
                .FirstOrDefaultAsync(x => x.PostID == id);

            if (p == null) return NotFound();

            return Ok(new PostDetailResponseDto
            {
                PostID        = p.PostID,
                Content       = p.Content,
                CreatedAt     = p.CreatedAt,
                AuthorId      = p.UserID,
                AuthorName    = p.User.FullName,
                Media         = p.Media.Select(m => new PostMediaResponseDto
                    { MediaID = m.MediaID, MediaURL = m.MediaURL, MediaType = m.MediaType }).ToList(),
                ReactionCount = p.Reactions.Count,
                CommentCount  = p.Comments.Count,
                Comments      = p.Comments.OrderByDescending(c => c.CreatedAt).Select(c => new CommentResponseDto
                {
                    CommentID     = c.CommentID,
                    Content       = c.Content,
                    CreatedAt     = c.CreatedAt,
                    AuthorId      = c.UserID,
                    AuthorName    = c.User.FullName,
                    Media         = c.Media.Select(m => new CommentMediaResponseDto
                        { MediaID = m.MediaID, MediaURL = m.MediaURL, MediaType = m.MediaType }).ToList(),
                    ReactionCount = c.Reactions.Count
                }).ToList()
            });
        }

        // ──────────────────────────────────────────────────────────────────────
        // MY POSTS
        // ──────────────────────────────────────────────────────────────────────
        [HttpGet("my")]
        public async Task<ActionResult<List<PostListResponseDto>>> GetMyPosts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetUserId();
            return Ok(await _context.Posts
                .Include(p => p.User).Include(p => p.Media)
                .Include(p => p.Reactions).Include(p => p.Comments)
                .Where(p => p.UserID == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => new PostListResponseDto
                {
                    PostID = p.PostID, Content = p.Content, CreatedAt = p.CreatedAt,
                    AuthorId = p.UserID, AuthorName = p.User.FullName,
                    Media = p.Media.Select(m => new PostMediaResponseDto
                        { MediaID = m.MediaID, MediaURL = m.MediaURL, MediaType = m.MediaType }).ToList(),
                    ReactionCount = p.Reactions.Count, CommentCount = p.Comments.Count
                }).ToListAsync());
        }

        // ──────────────────────────────────────────────────────────────────────
        // UPDATE
        // ──────────────────────────────────────────────────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto dto)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            if (post.UserID != GetUserId()) return Forbid();

            post.Content = dto.Content;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ──────────────────────────────────────────────────────────────────────
        // DELETE
        // ──────────────────────────────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.Include(p => p.Media).FirstOrDefaultAsync(p => p.PostID == id);
            if (post == null) return NotFound();
            if (post.UserID != GetUserId()) return Forbid();

            _context.Posts.Remove(post); // cascades media via EF config
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ──────────────────────────────────────────────────────────────────────
        private static PostListResponseDto MapToListDto(Post p, ApplicationUser user) => new()
        {
            PostID = p.PostID, Content = p.Content, CreatedAt = p.CreatedAt,
            AuthorId = p.UserID, AuthorName = user.FullName,
            Media = (p.Media ?? []).Select(m => new PostMediaResponseDto
                { MediaID = m.MediaID, MediaURL = m.MediaURL, MediaType = m.MediaType }).ToList(),
            ReactionCount = 0, CommentCount = 0
        };
    }
}
