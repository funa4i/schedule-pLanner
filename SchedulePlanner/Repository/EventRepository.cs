using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using SchedulePlannerBack.Domain;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces;

namespace SchedulePlannerBack.Repository;

public class EventRepository(SchedulePlannerDbContext context) : IEventRepository
{
    private readonly SchedulePlannerDbContext _context = context;
    public void Save(Event ev)
    {
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

    public void Update(Event ev)
    {
        try
        {
           _context.Update(ev);
        }
        catch (Exception e)
        {
            _context.ChangeTracker.Clear();
            throw new StorageException(e);
        }
    }
    
    
    public  Event? GetByLink(string link)
    {
        try
        {
            return _context
                .Events
                .Include(e => e.Participants)!
                .ThenInclude(p => p.EventDates)
                .FirstOrDefault(e => e.Link == link);
        }
        catch (Exception e)
        {
            _context.ChangeTracker.Clear();
            throw new StorageException(e);
        }
    }

    public List<Event> GetAllEventsWithUserId(long userId)
    {
        try
        {
            return _context.Events
                .Include(p => p.Participants)!.ThenInclude(p => p.EventDates)
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