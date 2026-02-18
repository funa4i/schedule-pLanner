using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces;
using SchedulePlannerBack.Interfaces.Application;
using SchedulePlannerBack.Repository;
using SchedulePlannerBack.Util;
using LinkGenerator = SchedulePlannerBack.Util.LinkGenerator;

namespace SchedulePlannerBack.Service;

public class EventService : IEventService
{
    private readonly ILogger _logger;
    private readonly IEventRepository _eventRepository;

    public EventService(ILogger logger, IEventRepository eventRepository)
    {
        _logger = logger;
        _eventRepository = eventRepository;
    }


    public Event CreateEvent(Event newEvent, List<EventDate> dates, long userId)
    {
        _logger.LogInformation("Creating new event by {UserId}", userId);
        newEvent.UserId = (long)userId!;
        newEvent.Link = LinkGenerator.GenerateLink();

        var paricipant = new Participant();
        paricipant.UserId = userId;
        paricipant.EventDates = dates;
        newEvent.Participants = [paricipant];
        _eventRepository.Save(newEvent);
        return newEvent;
    }

    public Event GetEvent(string eventLink)
    {
        _logger.LogInformation("Getting event by {EventLink}", eventLink);
        return _eventRepository.GetByLink(eventLink) ?? throw  new ElementNotFoundException(eventLink);
    }

    public List<Event> GetAllEventsWithUserId(long userId)
    {
        return _eventRepository.GetAllEventsWithUserId(userId).ToList();
    }

    public Event ResultEventTime(string eventLink, long userId)
    {
        _logger.LogInformation("Evaluate result time for event: {EventLink}", eventLink);
        var dbEvent = _eventRepository.GetByLink(eventLink) ?? throw new ElementNotFoundException(eventLink);
        if (dbEvent.UserId != userId)
        {
            throw new AuthorizationException("UserId does not match");
        }

        var allDates = dbEvent.Participants!.SelectMany(p => p!.EventDates).ToList();
        var dateTimeFrom = allDates.Min(x => x.DateFrom);
        var dateTimeTo = allDates.Min(x => x.DateTo);
        foreach (var date in allDates)
        {
            if (date.DateFrom.CompareTo(dateTimeTo) > 0)
            {
                dbEvent.TimeResult = "No time match";
                break;
            }

            dateTimeFrom = date.DateFrom;
        }

        if (dbEvent.TimeResult != "No time match")
        {
            dbEvent.TimeResult = "from: " + dateTimeFrom + " to: " + dateTimeTo;
        }
        _eventRepository.Update(dbEvent);
        return dbEvent;
    }

}