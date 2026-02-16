using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulePlannerBack.Adapters;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerBack.Service;

namespace SchedulePlannerBack.Controller;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ParticipantController : ControllerBase
{
    private readonly ParticipantAdapter _participantAdapter;

    public ParticipantController(ParticipantAdapter participantAdapter)
    {
        _participantAdapter = participantAdapter;
    }

    [HttpPost]
    public IActionResult SaveParticipant([FromBody] ParticipantRequest participantRequest)
    {
        return _participantAdapter.SaveParticipant(participantRequest).GetResponse(Request, Response);
    }
}