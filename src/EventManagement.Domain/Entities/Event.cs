namespace EventManagement.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Organizer { get; set; } = string.Empty;

    public List<Ticket> Tickets { get; set; } = [];
}
