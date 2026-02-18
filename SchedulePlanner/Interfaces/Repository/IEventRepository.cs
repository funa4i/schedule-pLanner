using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Interfaces;

public interface IEventRepository
{
    void Update(Event ev);
    void Save(Event ev);
    Event? GetByLink(string link);
    List<Event> GetAllEventsWithUserId(long userId);
    Event? GetById(long id);
}