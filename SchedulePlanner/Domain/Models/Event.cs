using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchedulePlannerBack.Domain.Entity.Enums;

namespace SchedulePlannerBack.Domain.Entity;

public class Event
{
    public long Id {get; set;}
    public long UserId {get; set;}
    [Column(TypeName = "varchar(256)")]
    public required string Title {get; set;}
    public EventDateType Type {get; set;}
    [Column(TypeName = "varchar(8)")]
    public required string Link {get; set;}
    public User? User {get; set;}
    [ForeignKey("EventId")]
    public List<Participant>? Participants {get; set;}
}