using SchedulePlannerBack.Domain.Bindings;

namespace SchedulePlannerBack.Domain.Views;

public class ParticipantView
{
    public ParticipantView(string guestName, List<EventDateView> eventDateViews)
    {
        GuestName = guestName;
        EventDateViews = eventDateViews;
    }

    public ParticipantView()
    {
        
    }

    public string GuestName {get; set;}
    List<EventDateView>  EventDateViews {get; set;}
}