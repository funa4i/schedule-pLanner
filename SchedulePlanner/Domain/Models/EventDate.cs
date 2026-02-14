namespace SchedulePlannerBack.Domain.Entity;

public class EventDate
{
    public long Id {get; set;}
    public long ParticipantId {get; set;}
    public DateTime DateFrom {get; set;}
    public DateTime DateTo {get; set;}
    public Participant? Participant {get; set;}
}