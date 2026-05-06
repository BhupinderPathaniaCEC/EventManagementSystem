using EventManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Event> Events { get; }
    DbSet<Ticket> Tickets { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
