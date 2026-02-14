using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulePlannerBack.Domain.Entity;

public class Participant
{
    public long Id {get; set;}
    public long EventId {get; set;}
    public long? UserId {get; set;}
    public string? GuestName {get; set;}
    public User? User {get; set;}
    public Event? Event {get; set;}
    [ForeignKey("ParticipantId")]
    public List<EventDate>? EventDates { get; set; }
}