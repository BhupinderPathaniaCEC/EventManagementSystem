using System.ComponentModel.DataAnnotations;

namespace EventManagement.Application.DTOs;

public class EventDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Organizer is required")]
    public string Organizer { get; set; } = string.Empty;

    public List<TicketDto> Tickets { get; set; } = [];
}
