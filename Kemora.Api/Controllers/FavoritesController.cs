using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kemora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FavoritesController(ApplicationDbContext context) => _context = context;

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("{placeId}")]
        public async Task<IActionResult> AddFavorite(int placeId)
        {
            if (!await _context.Places.AnyAsync(p => p.PlaceID == placeId))
                return NotFound("Place not found.");

            var userId = GetUserId();
            if (await _context.UserFavorites.AnyAsync(f => f.UserID == userId && f.PlaceID == placeId))
                return Conflict("Already favorited.");

            _context.UserFavorites.Add(new UserFavorite { UserID = userId, PlaceID = placeId });
            await _context.SaveChangesAsync();
            return StatusCode(201, new { message = "Favorited." });
        }

        [HttpDelete("{placeId}")]
        public async Task<IActionResult> RemoveFavorite(int placeId)
        {
            var userId = GetUserId();
            var fav = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.PlaceID == placeId);
            if (fav == null) return NotFound("Not in favorites.");
            _context.UserFavorites.Remove(fav);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<List<FavoriteResponseDto>>> GetMyFavorites()
        {
            var userId = GetUserId();
            return Ok(await _context.UserFavorites
                .Include(f => f.Place)
                .Where(f => f.UserID == userId)
                .Select(f => new FavoriteResponseDto
                {
                    PlaceID = f.PlaceID, PlaceName = f.Place.Name,
                    PlaceAddress = f.Place.Address, MainImageURL = f.Place.MainImageURL
                }).ToListAsync());
        }

        [HttpGet("{placeId}/check")]
        public async Task<ActionResult<FavoriteCheckDto>> CheckFavorite(int placeId)
        {
            var userId = GetUserId();
            var exists = await _context.UserFavorites
                .AnyAsync(f => f.UserID == userId && f.PlaceID == placeId);
            return Ok(new FavoriteCheckDto { IsFavorited = exists });
        }
    }
}
