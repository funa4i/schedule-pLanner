using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Interfaces.Application;

public interface IEventService
{
    Event CreateEvent(Event newEvent, List<EventDate> dates, long userId);
    Event GetEvent(string eventLink);
    Event ResultEventTime(string eventLink, long userId);
    List<Event> GetAllEventsWithUserId(long userId);
}