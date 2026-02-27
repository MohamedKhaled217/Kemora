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
    /// Community posts: create, browse, update, and delete social posts with media.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        /// <summary>
        /// Create a new community post.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PostListResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostListResponseDto>> CreatePost([FromBody] CreatePostDto dto)
        {
            var post = await _postService.CreateAsync(GetUserId(), dto);
            return Ok(post);
        }

        /// <summary>
        /// Browse all posts with pagination.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<PostListResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<PostListResponseDto>>> GetPosts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return Ok(await _postService.GetPostsAsync(page, pageSize));
        }

        /// <summary>
        /// Get a single post with full details.
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PostDetailResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDetailResponseDto>> GetPost(int id)
        {
            var p = await _postService.GetPostAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        /// <summary>
        /// Get the authenticated user's posts.
        /// </summary>
        [HttpGet("my")]
        [ProducesResponseType(typeof(PagedResult<PostListResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<PostListResponseDto>>> GetMyPosts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return Ok(await _postService.GetMyPostsAsync(GetUserId(), page, pageSize));
        }

        /// <summary>
        /// Update your own post.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto dto)
        {
            if (await _postService.UpdatePostAsync(id, GetUserId(), dto))
                return NoContent();
            return NotFound("Post not found or unauthorized.");
        }

        /// <summary>
        /// Delete your own post.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (await _postService.DeletePostAsync(id, GetUserId()))
                return NoContent();
            return NotFound("Post not found or unauthorized.");
        }
    }
}
