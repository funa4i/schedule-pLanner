using EntityFramework.Exceptions.Common;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Exceptions;

namespace SchedulePlannerBack.Domain.Repository;

public class ParticipantRepository(SchedulePlannerDbContext context)
{
    private readonly SchedulePlannerDbContext _context = context;
    
    public void Save(Participant participant)
    {
        try
        {  
            _context.Add(participant);
            _context.SaveChanges();
        }
        catch (UniqueConstraintException e)
        {
            throw new ElementExistsException(e.ConstraintName,
                e.ConstraintProperties.FirstOrDefault(""));
        } 
        catch (Exception e)
        {
            _context.ChangeTracker.Clear();
            throw new StorageException(e);
        }
    }
}