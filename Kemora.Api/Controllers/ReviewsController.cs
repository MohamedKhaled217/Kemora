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
    /// User reviews for places: submit, browse, and delete reviews with ratings.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/places/{placeId}/reviews")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private string GetUserName() => User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous";

        /// <summary>
        /// Submit a review for a place.
        /// </summary>
        /// <param name="placeId">The place to review.</param>
        /// <param name="dto">Review content and rating.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewResponseDto>> AddReview(int placeId, [FromBody] CreateReviewDto dto)
        {
            var rev = await _reviewService.CreateReviewAsync(GetUserId(), GetUserName(), placeId, dto);
            if (rev == null) return NotFound("Place not found.");
            return Ok(rev);
        }

        /// <summary>
        /// Get paginated reviews for a place.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<ReviewResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ReviewResponseDto>>> GetReviews(int placeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _reviewService.GetReviewsAsync(placeId, page, pageSize));
        }

        /// <summary>
        /// Delete your own review.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (await _reviewService.DeleteReviewAsync(id, GetUserName(), GetUserId()))
                return NoContent();
            return NotFound("Review not found or unauthorized.");
        }
    }
}
