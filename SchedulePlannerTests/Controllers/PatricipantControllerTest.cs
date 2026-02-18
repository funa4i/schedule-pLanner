using System.Net;
using Microsoft.EntityFrameworkCore;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerTests.Infrustructure;

namespace SchedulePlannerTests.Controllers;

[TestFixture]
public class ParticipantControllerTest : BaseWebApiControllerTest
{
    [TearDown]
    public void TearDown()
    {
        _context.RemoveEventDatesFromDatabase();
        _context.RemoveEventsFromDatabase();
        _context.RemoveParticipantFromDatabase();
        _context.SaveChanges();
    }

    [SetUp]
    public void OnSetup()
    {
        _context.RemoveEventDatesFromDatabase();
        _context.RemoveParticipantFromDatabase();
        _context.RemoveEventDatesFromDatabase();
        _context.SaveChanges();
    }

    [Test]
    public async Task Post_SaveParticipant_ValidData_ReturnsOk()
    {
        var us = _context.Users.First();
        
        var ev = _context.InsertEvent(us.Id, user: us);
        var request = new ParticipantRequest(
            eventId: ev.Id,
            guestName: "John Doe",
            eventDates: new List<EventDateRequest>
            {
                new EventDateRequest(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2))
            }
        );

        var response = await HttpClient.PostAsync(
            "/Participant",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var savedParticipant = _context.Participants.Include(participant => participant.EventDates).FirstOrDefault(p => p.GuestName == request.GuestName);
        
        Assert.That(savedParticipant, Is.Not.Null);
        Assert.That(savedParticipant.EventId, Is.EqualTo(ev.Id));
        Assert.That(savedParticipant.EventDates!.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task Post_SaveParticipant_InvalidDates_ReturnsBadRequest()
    {
        var request = new ParticipantRequest(
            eventId: 1,
            guestName: "Jane Doe",
            eventDates: new List<EventDateRequest>
            {
                new EventDateRequest(DateTime.UtcNow.AddDays(1), DateTime.UtcNow)
            }
        );

        var response = await HttpClient.PostAsync(
            "/Participant",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task Post_SaveParticipant_EmptyGuestName_ReturnsBadRequest()
    {
        var ev = _context.InsertEvent(_context.Users.First().Id);
        var request = new ParticipantRequest(
            eventId: ev.Id,
            guestName: "",
            eventDates: new List<EventDateRequest>
            {
                new EventDateRequest(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2))
            }
        );

        var response = await HttpClient.PostAsync(
            "/Participant",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_SaveParticipant_EmptyEventDates_ReturnsBadRequest()
    {
        var ev = _context.InsertEvent(_context.Users.First().Id);
        var request = new ParticipantRequest(
            eventId: ev.Id,
            guestName: "Test Guest",
            eventDates: new List<EventDateRequest>()
        );

        var response = await HttpClient.PostAsync(
            "/Participant",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_SaveParticipant_InvalidEventId_ReturnsBadRequest()
    {
        var request = new ParticipantRequest(
            eventId: -1, 
            guestName: "Test Guest",
            eventDates: new List<EventDateRequest>
            {
                new EventDateRequest(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2))
            }
        );

        var response = await HttpClient.PostAsync(
            "/Participant",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
