using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kemora.Api.Controllers
{
    [Route("api/places/{placeId}/photos")]
    [ApiController]
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PhotosController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<PhotoResponseDto>> AddPhoto(int placeId, [FromBody] CreatePhotoDto dto)
        {
            if (!await _context.Places.AnyAsync(p => p.PlaceID == placeId))
                return NotFound("Place not found.");

            // If this is marked as main, un‑main existing ones
            if (dto.IsMain)
            {
                var existing = await _context.Photos
                    .Where(p => p.PlaceID == placeId && p.IsMain).ToListAsync();
                existing.ForEach(p => p.IsMain = false);
            }

            var photo = new Photo { ImageURL = dto.ImageURL, IsMain = dto.IsMain, PlaceID = placeId };
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPhotos), new { placeId },
                new PhotoResponseDto { PhotoID = photo.PhotoID, ImageURL = photo.ImageURL, IsMain = photo.IsMain, PlaceID = placeId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<PhotoResponseDto>>> GetPhotos(int placeId)
        {
            return Ok(await _context.Photos
                .Where(p => p.PlaceID == placeId)
                .Select(p => new PhotoResponseDto { PhotoID = p.PhotoID, ImageURL = p.ImageURL, IsMain = p.IsMain, PlaceID = p.PlaceID })
                .ToListAsync());
        }

        [HttpPut("/api/photos/{id}/set-main")]
        public async Task<IActionResult> SetMainPhoto(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null) return NotFound();

            // Un‑main siblings
            var siblings = await _context.Photos
                .Where(p => p.PlaceID == photo.PlaceID && p.IsMain).ToListAsync();
            siblings.ForEach(p => p.IsMain = false);

            photo.IsMain = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("/api/photos/{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null) return NotFound();
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
