using Kemora.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface IEventService
    {
        Task<EventResponseDto?> CreateEventAsync(int placeId, CreateEventDto dto);
        Task<List<EventResponseDto>> GetUpcomingEventsAsync();
        Task<List<EventResponseDto>> GetPlaceEventsAsync(int placeId);
        Task<bool> UpdateEventAsync(int id, UpdateEventDto dto);
        Task<bool> DeleteEventAsync(int id);
    }
}
