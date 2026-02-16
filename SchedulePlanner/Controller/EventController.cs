using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulePlannerBack.Adapters;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerBack.Domain.Views;

namespace SchedulePlannerBack.Controller;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly EventAdapter _eventAdapter;

    public EventController(EventAdapter eventAdapter)
    {
        _eventAdapter = eventAdapter;
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetAllEvents()
    {
        return _eventAdapter.GetAllEventsWithUserId().GetResponse(Request, Response);
    }

    [HttpPost]
    [Authorize]
    public IActionResult CreateEvent([FromBody] EventRequest eventRequest)
    {
        return _eventAdapter.CreateEvent(eventRequest).GetResponse(Request, Response);
    }

    [HttpGet("{link}")]
    public IActionResult GetEvent(string link)
    {
        return _eventAdapter.GetEventWithLink(link).GetResponse(Request, Response);
    }

    [HttpGet("/{link}/optimal")]
    public IActionResult GetOptimal(string link)
    {
        return _eventAdapter.ResultEventTime(link).GetResponse(Request, Response);
    }
}