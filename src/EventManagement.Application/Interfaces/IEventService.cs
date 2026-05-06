using EventManagement.Application.DTOs;

namespace EventManagement.Application.Interfaces;

public interface IEventService
{
    Task<List<EventDto>> GetAllEventsAsync();
    Task<EventDto?> GetEventByIdAsync(int id);
    Task CreateEventAsync(EventDto dto);
    Task UpdateEventAsync(EventDto dto);
    Task DeleteEventAsync(int id);
}
