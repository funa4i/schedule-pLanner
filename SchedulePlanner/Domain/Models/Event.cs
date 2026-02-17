using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchedulePlannerBack.Domain.Entity.Enums;

namespace SchedulePlannerBack.Domain.Entity;

[Table("user_events")]
public class Event
{
    public long Id {get; set;}
    public long UserId {get; set;}
    [Column(TypeName = "varchar(256)")]
    public required string Title {get; set;}
    public required EventDateType Type {get; set;}
    [Column(TypeName = "varchar(8)")]
    public string? Link {get; set;}
    [Column(TypeName = "varchar(256)")] 
    public required string TimeResult { get; set; } = ""; 
    public User? User {get; set;}
    [ForeignKey("EventId")]
    public List<Participant>? Participants {get; set;}
}