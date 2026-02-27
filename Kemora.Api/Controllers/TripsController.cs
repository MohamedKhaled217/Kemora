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
    public class TripsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        public TripsController(ApplicationDbContext ctx) => _ctx = ctx;
        private string UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost]
        public async Task<ActionResult<TripDetailDto>> Create([FromBody] CreateTripDto dto)
        {
            if (dto.EndDate <= dto.StartDate) return BadRequest("EndDate must be after StartDate.");
            var t = new Trip { Name = dto.Name, Description = dto.Description, StartDate = dto.StartDate, EndDate = dto.EndDate, UserID = UserId() };
            _ctx.Trips.Add(t); await _ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = t.TripID },
                new TripDetailDto { TripID = t.TripID, Name = t.Name, Description = t.Description, StartDate = t.StartDate, EndDate = t.EndDate });
        }

        [HttpGet]
        public async Task<ActionResult<List<TripListDto>>> List([FromQuery] int page = 1, [FromQuery] int ps = 20)
        {
            return Ok(await _ctx.Trips.Where(t => t.UserID == UserId()).OrderByDescending(t => t.StartDate)
                .Skip((page - 1) * ps).Take(ps)
                .Select(t => new TripListDto { TripID = t.TripID, Name = t.Name, StartDate = t.StartDate, EndDate = t.EndDate, PlaceCount = t.TripPlaces.Count })
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDetailDto>> Get(int id)
        {
            var t = await _ctx.Trips.Include(x => x.TripPlaces).ThenInclude(tp => tp.Place).FirstOrDefaultAsync(x => x.TripID == id);
            if (t == null) return NotFound();
            if (t.UserID != UserId()) return Forbid();
            return Ok(new TripDetailDto
            {
                TripID = t.TripID, Name = t.Name, Description = t.Description, StartDate = t.StartDate, EndDate = t.EndDate,
                Places = t.TripPlaces.OrderBy(tp => tp.VisitDate).Select(tp => new TripPlaceResponseDto
                { TripPlaceID = tp.TripPlaceID, PlaceID = tp.PlaceID, PlaceName = tp.Place.Name, VisitDate = tp.VisitDate, Notes = tp.Notes }).ToList()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTripDto dto)
        {
            var t = await _ctx.Trips.FindAsync(id);
            if (t == null) return NotFound();
            if (t.UserID != UserId()) return Forbid();
            if (dto.Name != null) t.Name = dto.Name;
            if (dto.Description != null) t.Description = dto.Description;
            if (dto.StartDate.HasValue) t.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) t.EndDate = dto.EndDate.Value;
            await _ctx.SaveChangesAsync(); return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _ctx.Trips.FindAsync(id);
            if (t == null) return NotFound();
            if (t.UserID != UserId()) return Forbid();
            _ctx.Trips.Remove(t); await _ctx.SaveChangesAsync(); return NoContent();
        }

        [HttpPost("{id}/places")]
        public async Task<ActionResult<TripPlaceResponseDto>> AddPlace(int id, [FromBody] AddTripPlaceDto dto)
        {
            var t = await _ctx.Trips.FindAsync(id);
            if (t == null) return NotFound();
            if (t.UserID != UserId()) return Forbid();
            var place = await _ctx.Places.FindAsync(dto.PlaceID);
            if (place == null) return BadRequest("Place not found.");
            if (await _ctx.TripPlaces.AnyAsync(tp => tp.TripID == id && tp.PlaceID == dto.PlaceID)) return Conflict("Already added.");
            var tp = new TripPlace { TripID = id, PlaceID = dto.PlaceID, VisitDate = dto.VisitDate, Notes = dto.Notes };
            _ctx.TripPlaces.Add(tp); await _ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id },
                new TripPlaceResponseDto { TripPlaceID = tp.TripPlaceID, PlaceID = tp.PlaceID, PlaceName = place.Name, VisitDate = tp.VisitDate, Notes = tp.Notes });
        }

        [HttpPut("{id}/places/{tpId}")]
        public async Task<IActionResult> UpdatePlace(int id, int tpId, [FromBody] UpdateTripPlaceDto dto)
        {
            var t = await _ctx.Trips.FindAsync(id);
            if (t == null) return NotFound();
            if (t.UserID != UserId()) return Forbid();
            var tp = await _ctx.TripPlaces.FindAsync(tpId);
            if (tp == null || tp.TripID != id) return NotFound();
            if (dto.VisitDate.HasValue) tp.VisitDate = dto.VisitDate.Value;
            if (dto.Notes != null) tp.Notes = dto.Notes;
            await _ctx.SaveChangesAsync(); return NoContent();
        }

        [HttpDelete("{id}/places/{tpId}")]
        public async Task<IActionResult> RemovePlace(int id, int tpId)
        {
            var t = await _ctx.Trips.FindAsync(id);
            if (t == null) return NotFound();
            if (t.UserID != UserId()) return Forbid();
            var tp = await _ctx.TripPlaces.FindAsync(tpId);
            if (tp == null || tp.TripID != id) return NotFound();
            _ctx.TripPlaces.Remove(tp); await _ctx.SaveChangesAsync(); return NoContent();
        }
    }
}
