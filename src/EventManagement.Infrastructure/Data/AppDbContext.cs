using EventManagement.Application.Interfaces;
using EventManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Data;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Organizer).IsRequired().HasMaxLength(100);
            e.HasMany(x => x.Tickets)
             .WithOne(t => t.Event)
             .HasForeignKey(t => t.EventId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ticket>(t =>
        {
            t.HasKey(x => x.Id);
            t.Property(x => x.TicketNo).IsRequired().HasMaxLength(50);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
