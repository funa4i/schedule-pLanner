using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Entity.Enums;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces;
using SchedulePlannerBack.Service;

namespace SchedulePlannerTests.ApplicationTests;

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

[TestFixture]
public class EventServiceTest
{
    private Mock<ILogger> _loggerMock;
    private Mock<IEventRepository> _eventRepositoryMock;
    private EventService _eventService;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _eventService = new EventService(
            _loggerMock.Object,
            _eventRepositoryMock.Object);
    }
    
    [Test]
    public void CreateEvent_SetsUserIdAndLink_AndCallsSave()
    {
        var ev = CreateEvent();
        var dates = new List<EventDate> { CreateEventDate() };
        long userId = 5;

        var result = _eventService.CreateEvent(ev, dates, userId);

        _eventRepositoryMock.Verify(x => x.Save(ev), Times.Once);
        Assert.That(userId, Is.EqualTo(result.UserId));
        Assert.That(result.Link, Is.Not.Null);
    }
    
    [Test]
    public void GetEvent_ReturnsEvent_WhenExists()
    {
        var ev = CreateEvent();
        string link = "abc";

        _eventRepositoryMock
            .Setup(x => x.GetByLink(link))
            .Returns(ev);

        var result = _eventService.GetEvent(link);

        Assert.That(ev, Is.EqualTo(result));
        _eventRepositoryMock.Verify(x => x.GetByLink(link), Times.Once);
    }

    [Test]
    public void GetEvent_ThrowsElementNotFoundException_WhenNull()
    {
        _eventRepositoryMock
            .Setup(x => x.GetByLink(It.IsAny<string>()))
            .Returns((Event?)null);

        Assert.Throws<ElementNotFoundException>(() =>
            _eventService.GetEvent("badlink"));
    }

    [Test]
    public void GetAllEventsWithUserId_CallsRepository()
    {
        long userId = 3;
        var list = new List<Event> { CreateEvent() };

        _eventRepositoryMock
            .Setup(x => x.GetAllEventsWithUserId(userId))
            .Returns(list);

        var result = _eventService.GetAllEventsWithUserId(userId);

        Assert.That(list, Is.EqualTo(result));
        _eventRepositoryMock.Verify(
            x => x.GetAllEventsWithUserId(userId),
            Times.Once);
    }


    [Test]
    public void ResultEventTime_ThrowsElementNotFoundException_WhenEventNotFound()
    {
        _eventRepositoryMock
            .Setup(x => x.GetByLink(It.IsAny<string>()))
            .Returns((Event?)null);

        Assert.Throws<ElementNotFoundException>(() =>
            _eventService.ResultEventTime("link", 1));
    }

    [Test]
    public void ResultEventTime_ThrowsAuthorizationException_WhenUserIdMismatch()
    {
        var ev = CreateEvent();
        ev.UserId = 10;

        _eventRepositoryMock
            .Setup(x => x.GetByLink(It.IsAny<string>()))
            .Returns(ev);

        Assert.Throws<AuthorizationException>(() =>
            _eventService.ResultEventTime("link", 5));
    }

    [Test]
    public void ResultEventTime_SetsNoTimeMatch_WhenDatesDoNotOverlap()
    {
        var ev = CreateEvent();
        ev.UserId = 1;
        ev.Participants = new List<Participant>
        {
            new Participant
            {
                EventDates = new List<EventDate>
                {
                    new EventDate
                    {
                        DateFrom = DateTime.UtcNow.AddDays(1),
                        DateTo = DateTime.UtcNow.AddDays(2)
                    },
                    new EventDate
                    {
                        DateFrom = DateTime.UtcNow.AddDays(3),
                        DateTo = DateTime.UtcNow.AddDays(4)
                    }
                }
            }
        };

        _eventRepositoryMock
            .Setup(x => x.GetByLink("link"))
            .Returns(ev);

        var result = _eventService.ResultEventTime("link", 1);

        Assert.That("No time match", Is.EqualTo(result.TimeResult));
        _eventRepositoryMock.Verify(x => x.Update(ev), Times.Once);
    }

    [Test]
    public void ResultEventTime_SetsTimeRange_WhenDatesOverlap()
    {
        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow.AddHours(5);

        var ev = CreateEvent();
        ev.UserId = 1;
        ev.Participants = new List<Participant>
        {
            new Participant
            {
                EventDates = new List<EventDate>
                {
                    new EventDate { DateFrom = from, DateTo = to },
                    new EventDate { DateFrom = from.AddHours(1), DateTo = to }
                }
            }
        };

        _eventRepositoryMock
            .Setup(x => x.GetByLink("link"))
            .Returns(ev);

        var result = _eventService.ResultEventTime("link", 1);

        Assert.That(result.TimeResult!.StartsWith("from:"));
        _eventRepositoryMock.Verify(x => x.Update(ev), Times.Once);
    }
    private Event CreateEvent()
    {
        return new Event
        {
            Id = 1,
            UserId = 1,
            Participants = new List<Participant>(),
            Title = "SomeTitile",
            Type = (EventDateType)0,
            TimeResult = ""
        };
    }

    private EventDate CreateEventDate()
    {
        return new EventDate
        {
            DateFrom = DateTime.UtcNow,
            DateTo = DateTime.UtcNow.AddHours(1)
        };
    }
}
