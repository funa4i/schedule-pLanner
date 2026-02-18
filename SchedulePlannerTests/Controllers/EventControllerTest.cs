using System.Net;
using Microsoft.EntityFrameworkCore;
using SchedulePlannerBack.Domain.Bindings;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Entity.Enums;
using SchedulePlannerBack.Domain.Views;
using SchedulePlannerTests.Infrustructure;

namespace SchedulePlannerTests.Controllers;

[TestFixture]
public class EventControllerTest : BaseWebApiControllerTest
{
    [SetUp]
    public void OnSetup()
    {
        _context.RemoveEventDatesFromDatabase();
        _context.RemoveParticipantFromDatabase();
        _context.RemoveEventDatesFromDatabase();
        _context.SaveChanges();
    } 
    [TearDown]
    public void TearDown()
    {
        _context.RemoveEventDatesFromDatabase();
        _context.RemoveEventsFromDatabase();
        _context.RemoveParticipantFromDatabase();
        _context.SaveChanges();
    }


    [Test]
    public async Task Get_GetAllEvents_WhenHaveRecords_ReturnsOk()
    {
        var users = _context.Users.ToList();
        var user = _context.Users.First();
        _context.InsertEvent(user.Id, user: user);
        _context.InsertEvent(user.Id, user: user);

        var response = await HttpClient.GetAsync("/Event");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await GetModelFromResponseAsync<List<EventView>>(response);

        Assert.That(data, Is.Not.Null);
        Assert.That(data, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Get_GetAllEvents_WhenNoRecords_ReturnsEmpty()
    {
        var user = _context.Users.First();
        

        var response = await HttpClient.GetAsync("/Event");
        var data = await GetModelFromResponseAsync<List<EventView>>(response);

        Assert.That(data, Is.Not.Null);
        Assert.That(data, Is.Empty);
    }

    [Test]
    public async Task Post_CreateEvent_ValidMultipleDates_ReturnsOk()
    {
        var user = _context.Users.First();
        

        var request = new EventRequest(
            title: "Test Event",
            type: EventDateType.Multiple,
            eventDateRequests: new List<EventDateRequest>
            {
                new EventDateRequest(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)),
                new EventDateRequest(DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(4))
            }
        );

        var response = await HttpClient.PostAsync(
            "/Event",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var ev = _context.Events
            .Include(e => e.Participants)!
            .ThenInclude(p => p.EventDates)
            .FirstOrDefault(x => x.Title == request.Title);
        
        
        Assert.That(ev, Is.Not.Null);
        Assert.That(ev!.Participants!.SelectMany(p => p.EventDates!).Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Post_CreateEvent_SingleWithMultipleDates_ReturnsBadRequest()
    {
        var user = _context.Users.First();
        

        var request = new EventRequest(
            title: "Single Event",
            type: EventDateType.Single,
            eventDateRequests: new List<EventDateRequest>
            {
                new EventDateRequest(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)),
                new EventDateRequest(DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(4))
            }
        );

        var response = await HttpClient.PostAsync(
            "/Event",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_CreateEvent_EmptyTitle_ReturnsBadRequest()
    {
        var user = _context.Users.First();
        

        var request = new EventRequest(
            title: "",
            type: EventDateType.Single,
            eventDateRequests: new List<EventDateRequest>
            {
                new EventDateRequest(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2))
            }
        );

        var response = await HttpClient.PostAsync(
            "/Event",
            MakeContent(request)
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Get_GetEvent_ByLink_ReturnsOk()
    {
        var user = _context.Users.First();
        var ev = _context.InsertEvent(user.Id);

        var response = await HttpClient.GetAsync($"/Event/{ev.Link}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await GetModelFromResponseAsync<EventView>(response);

        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Id, Is.EqualTo(ev.Id));
    }

    [Test]
    public async Task Get_GetEvent_WhenNotFound_ReturnsNotFound()
    {
        var response = await HttpClient.GetAsync("/Event/unknown-link");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Get_GetOptimal_WhenLinkExists_ReturnsOk()
    {
        var user = _context.Users.First();
        var ev = _context.InsertEvent(user.Id);
        var part = _context.InsertParticipant(eventId:ev.Id, eventDates: [new EventDate(){DateFrom = DateTime.UtcNow.AddDays(1), DateTo = DateTime.UtcNow.AddDays(2)}]);

        var response = await HttpClient.GetAsync($"/Event/{ev.Link}/optimal");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Get_GetOptimal_WhenLinkNotFound_ReturnsNotFound()
    {
        var response = await HttpClient.GetAsync("/Event/unknown/optimal");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}