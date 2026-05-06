using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Web.Controllers;

public class EventsController : Controller
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _eventService.GetAllEventsAsync();
        return View(events);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _eventService.CreateEventAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _eventService.GetEventByIdAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EventDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _eventService.UpdateEventAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _eventService.DeleteEventAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
