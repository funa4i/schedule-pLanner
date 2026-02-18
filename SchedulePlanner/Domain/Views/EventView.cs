using SchedulePlannerBack.Domain.Entity.Enums;

namespace SchedulePlannerBack.Domain.Views;

public class EventView
{
    public EventView(long id, string title, EventDateType type, string timeResult, string authorName, string link, List<ParticipantView> participants)
    {
        Id = id;
        Title = title;
        Type = type;
        TimeResult = timeResult;
        AuthorName = authorName;
        Link = link;
        Participants = participants;
    }

    public EventView()
    {
    }

    public long Id { get; set; }
    public string Title { get; set; }
    public EventDateType Type { get; set; }
    public string TimeResult {get; set;}
    public string AuthorName {get; set;}
    public string Link {get; set;}
    
    public List<ParticipantView> Participants {get; set;}
    
}