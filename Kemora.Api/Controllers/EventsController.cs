using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kemora.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EventsController(ApplicationDbContext context) => _context = context;

        [HttpPost("places/{placeId}/events")]
        public async Task<ActionResult<EventResponseDto>> CreateEvent(int placeId, [FromBody] CreateEventDto dto)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null) return NotFound("Place not found.");
            if (dto.EndDate <= dto.StartDate) return BadRequest("EndDate must be after StartDate.");

            var ev = new Event { Name = dto.Name, StartDate = dto.StartDate, EndDate = dto.EndDate, PlaceID = placeId };
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventsByPlace), new { placeId },
                new EventResponseDto
                {
                    EventID = ev.EventID, Name = ev.Name, StartDate = ev.StartDate,
                    EndDate = ev.EndDate, PlaceID = placeId, PlaceName = place.Name
                });
        }

        [HttpGet("events")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EventResponseDto>>> GetUpcomingEvents(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return Ok(await _context.Events
                .Include(e => e.Place)
                .Where(e => e.EndDate >= DateTime.UtcNow)
                .OrderBy(e => e.StartDate)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(e => new EventResponseDto
                {
                    EventID = e.EventID, Name = e.Name, StartDate = e.StartDate,
                    EndDate = e.EndDate, PlaceID = e.PlaceID, PlaceName = e.Place.Name
                }).ToListAsync());
        }

        [HttpGet("places/{placeId}/events")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EventResponseDto>>> GetEventsByPlace(int placeId)
        {
            if (!await _context.Places.AnyAsync(p => p.PlaceID == placeId))
                return NotFound("Place not found.");

            return Ok(await _context.Events
                .Include(e => e.Place)
                .Where(e => e.PlaceID == placeId)
                .OrderBy(e => e.StartDate)
                .Select(e => new EventResponseDto
                {
                    EventID = e.EventID, Name = e.Name, StartDate = e.StartDate,
                    EndDate = e.EndDate, PlaceID = e.PlaceID, PlaceName = e.Place.Name
                }).ToListAsync());
        }

        [HttpPut("events/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto dto)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            if (dto.Name != null) ev.Name = dto.Name;
            if (dto.StartDate.HasValue) ev.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) ev.EndDate = dto.EndDate.Value;
            if (ev.EndDate <= ev.StartDate) return BadRequest("EndDate must be after StartDate.");
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("events/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
