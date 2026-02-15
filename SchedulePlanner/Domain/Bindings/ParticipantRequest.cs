using System.ComponentModel.DataAnnotations;
using SchedulePlannerBack.Util;

namespace SchedulePlannerBack.Domain.Bindings;

public class ParticipantRequest : IValidation
{
    public ParticipantRequest(long eventId, string guestName, List<EventDateRequest> eventDates)
    {
        EventId = eventId;
        GuestName = guestName;
        EventDates = eventDates;
    }

    public long EventId { get; set; }
    public string GuestName { get; set; }
    public List<EventDateRequest> EventDates { get; set; }
    
    
    public void Validate()
    {
        if (EventId <= 0)
            throw new ValidationException("EventId must be positive");
        if (string.IsNullOrEmpty(GuestName))
            throw new ValidationException("GuestName is null or empty");
        if (EventDates.Count <= 0)
            throw new ValidationException("EventDates is empty");
        EventDates.ForEach(e => e.Validate());
    }
}