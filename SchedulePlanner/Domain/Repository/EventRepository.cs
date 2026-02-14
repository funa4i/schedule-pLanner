using EntityFramework.Exceptions.Common;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Exceptions;

namespace SchedulePlannerBack.Domain.Repository;

public class EventRepository(SchedulePlannerDbContext context)
{
    private readonly SchedulePlannerDbContext _context = context;
    public void Save(Event ev){
        try
        {
                _context.Add(ev);
                _context.SaveChanges();
        }
        catch (UniqueConstraintException ex) 
        {
                _context.ChangeTracker.Clear();
                throw new ElementExistsException(ex.ConstraintName, ex.ConstraintProperties.FirstOrDefault(""));
        }
        catch (Exception ex) 
        {
                _context.ChangeTracker.Clear();
                throw new StorageException(ex); 
        }
    }

    public List<Event> GetAllEventsWithUserId(long userId)
    {
        try
        {
            return _context.Events
                .Where(e => e.UserId == userId || e.Participants!.Any(p => p.UserId == userId))
                .ToList();
        }
        catch (Exception e)
        {
            _context.ChangeTracker.Clear();
            throw new StorageException(e);
        }
    }

    public Event? GetById(long id)
    {
        try
        {
            return _context.Events.FirstOrDefault(e => e.Id == id);
        }
        catch (Exception e)
        {
            _context.ChangeTracker.Clear();
            throw new StorageException(e);
        }
    }
    
}