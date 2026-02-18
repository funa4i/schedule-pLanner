using Microsoft.EntityFrameworkCore;
using SchedulePlannerBack.Domain;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Entity.Enums;

namespace SchedulePlannerTests.Infrustructure;

public static class SchedulePlannerDbExtension
{
    public static User InsertUser(this SchedulePlannerDbContext context, long? id = null, string? username = "Test", string? password = "Test")
    {
        var user = new User(){Id = id ?? 0, Login = username, Password = password};
        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    public static Participant InsertParticipant(this SchedulePlannerDbContext context, long eventId, long? id = null,
        long? userId = null, string? guestName = null, List<EventDate>? eventDates = null)
    {
        var participant = new Participant()
        {
            Id = id ?? 0,
            EventId = eventId,
            UserId = userId,
            GuestName = guestName,
            EventDates = eventDates ?? new List<EventDate>()
        };
        context.Participants.Add(participant);
        context.SaveChanges();
        return participant;
    }

    public static Event InsertEvent(this SchedulePlannerDbContext context, long userId, long? id = null,
        string title = "TestTitle", EventDateType type = EventDateType.Single, string? link = "123",
        string timeResult = "", List<Participant>? participants = null, User? user = null)
    {
        var ev = new Event()
        {
            Id = id ?? 0,
            UserId = userId,
            Title = title,
            Type = type,
            Link = link,
            TimeResult = timeResult,
            Participants = participants ?? new List<Participant>(),
            User = user
        };
        context.Events.Add(ev);
        context.SaveChanges();
        return ev;
    }
    
    public static void RemoveEventsFromDatabase(this SchedulePlannerDbContext
        dbContext) => dbContext.Events.RemoveRange(dbContext.Events);
    public static void RemoveEventDatesFromDatabase(this SchedulePlannerDbContext
        dbContext) => dbContext.EventDates.RemoveRange(dbContext.EventDates);
    public static void RemoveParticipantFromDatabase(this SchedulePlannerDbContext
        dbContext) => dbContext.Participants.RemoveRange(dbContext.Participants);
    public static void RemoveUsersFromDatabase(this SchedulePlannerDbContext
        dbContext) => dbContext.Users.RemoveRange(dbContext.Users);
}