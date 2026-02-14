using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulePlannerBack.Domain.Entity;

public class User
{
    public long Id {get; set;}
    public required  string Name {get; set;}
    public required string Password {get; set;}
    [ForeignKey("UserId")]
    public List<Event> Events {get; set;}
    [ForeignKey("UserId")]
    public List<Participant> Participants {get; set;}
}