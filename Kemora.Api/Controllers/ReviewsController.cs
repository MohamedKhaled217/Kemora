using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kemora.Api.Controllers
{
    [Route("api/places/{placeId}/reviews")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReviewsController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<ReviewResponseDto>> CreateReview(
            int placeId, [FromBody] CreateReviewDto dto)
        {
            if (!await _context.Places.AnyAsync(p => p.PlaceID == placeId))
                return NotFound("Place not found.");

            var userName = User.Identity?.Name ?? "Anonymous";
            var review = new Review
            {
                AuthorName = userName,
                Rating     = dto.Rating,
                Text       = dto.Text,
                PlaceID    = placeId
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReviews), new { placeId },
                new ReviewResponseDto
                {
                    ReviewID = review.ReviewID, AuthorName = review.AuthorName,
                    Rating = review.Rating, Text = review.Text, PlaceID = placeId
                });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReviewResponseDto>>> GetReviews(
            int placeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (!await _context.Places.AnyAsync(p => p.PlaceID == placeId))
                return NotFound("Place not found.");

            var reviews = await _context.Reviews
                .Where(r => r.PlaceID == placeId)
                .OrderByDescending(r => r.ReviewID)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(r => new ReviewResponseDto
                {
                    ReviewID = r.ReviewID, AuthorName = r.AuthorName,
                    Rating = r.Rating, Text = r.Text, PlaceID = r.PlaceID
                }).ToListAsync();

            return Ok(reviews);
        }

        [HttpDelete("/api/reviews/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            // Only author (by name match) can delete – or admin in future
            if (review.AuthorName != (User.Identity?.Name ?? ""))
                return Forbid();
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
