namespace EventManagement.Application.DTOs;

public class TicketDto
{
    public int Id { get; set; }
    public string TicketNo { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
