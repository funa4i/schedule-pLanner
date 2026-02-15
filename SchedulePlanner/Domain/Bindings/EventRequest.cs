using SchedulePlannerBack.Domain.Entity.Enums;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Util;

namespace SchedulePlannerBack.Domain.Bindings;

public class EventRequest : IValidation
{
    public EventRequest(string title, EventDateType type, List<EventDateRequest> eventDateRequests)
    {
        Title = title;
        Type = type;
        EventDateRequests = eventDateRequests;
    }

    public string Title {get; set;}
    public EventDateType Type {get; set;}
    public List<EventDateRequest> EventDateRequests {get; set;}
    public void Validate()
    {
        if (string.IsNullOrEmpty(Title))
            throw new ValidationException("Title is null or empty");
        if (EventDateRequests.Count == 0)
            throw new ValidationException("EventDateRequests is empty");
        if (Type == EventDateType.Single &&  EventDateRequests.Count != 1)
            throw new ValidationException("Event with fixed type must have only one date");
        EventDateRequests.ForEach(e =>  e.Validate());
    }
}