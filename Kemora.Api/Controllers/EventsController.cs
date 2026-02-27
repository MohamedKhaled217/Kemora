using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Api.Controllers
{
    /// <summary>
    /// Manage events for places: create, list upcoming, update, and delete.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Create an event for a specific place (Admin only).
        /// </summary>
        [HttpPost("places/{placeId}/events")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventResponseDto>> CreateEvent(int placeId, [FromBody] CreateEventDto dto)
        {
            var response = await _eventService.CreateEventAsync(placeId, dto);
            if (response == null) return NotFound("Place not found.");
            return Ok(response);
        }

        /// <summary>
        /// Get upcoming events across all places.
        /// </summary>
        [HttpGet("events")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<EventResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EventResponseDto>>> GetUpcomingEvents()
        {
            return Ok(await _eventService.GetUpcomingEventsAsync());
        }

        /// <summary>
        /// Get events for a specific place.
        /// </summary>
        [HttpGet("places/{placeId}/events")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<EventResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EventResponseDto>>> GetEventsByPlace(int placeId)
        {
            return Ok(await _eventService.GetPlaceEventsAsync(placeId));
        }

        /// <summary>
        /// Update an existing event (Admin only).
        /// </summary>
        [HttpPut("events/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto dto)
        {
            if (await _eventService.UpdateEventAsync(id, dto)) return NoContent();
            return NotFound();
        }

        /// <summary>
        /// Delete an event (Admin only).
        /// </summary>
        [HttpDelete("events/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (await _eventService.DeleteEventAsync(id)) return NoContent();
            return NotFound();
        }
    }
}
