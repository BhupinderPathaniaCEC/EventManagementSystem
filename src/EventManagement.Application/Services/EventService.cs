using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using EventManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Application.Services;

public class EventService : IEventService
{
    private readonly IApplicationDbContext _context;

    public EventService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventDto>> GetAllEventsAsync()
    {
        return await _context.Events
            .Include(e => e.Tickets)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        var entity = await _context.Events
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null ? null : MapToDto(entity);
    }

    public async Task CreateEventAsync(EventDto dto)
    {
        var entity = MapToEntity(dto);
        _context.Events.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(EventDto dto)
    {
        var entity = await _context.Events
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == dto.Id);

        if (entity is null) return;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Organizer = dto.Organizer;

        _context.Tickets.RemoveRange(entity.Tickets);
        entity.Tickets = dto.Tickets.Select(t => new Ticket
        {
            TicketNo = t.TicketNo,
            Price = t.Price,
            EventId = entity.Id
        }).ToList();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(int id)
    {
        var entity = await _context.Events.FindAsync(id);
        if (entity is null) return;

        _context.Events.Remove(entity);
        await _context.SaveChangesAsync();
    }

    private static EventDto MapToDto(Event e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        Organizer = e.Organizer,
        Tickets = e.Tickets.Select(t => new TicketDto
        {
            Id = t.Id,
            TicketNo = t.TicketNo,
            Price = t.Price
        }).ToList()
    };

    private static Event MapToEntity(EventDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Organizer = dto.Organizer,
        Tickets = dto.Tickets.Select(t => new Ticket
        {
            TicketNo = t.TicketNo,
            Price = t.Price
        }).ToList()
    };
}
