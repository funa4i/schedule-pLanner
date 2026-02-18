using AutoMapper;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces;

namespace SchedulePlannerBack.Domain.Repository;

public class UserRepository(SchedulePlannerDbContext context) : IUserRepository
{
    private readonly SchedulePlannerDbContext _context = context;

    public User? GetByName(string name)
    {
        try
        {
            return _context.Users.FirstOrDefault(u => u.Login == name);
        }
        catch (Exception e)
        {
            _context.ChangeTracker.Clear();
            throw new StorageException(e);
        }
    }

    public User? GetById(long id)
    {
        try
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        catch (Exception e)

        {
            _context.ChangeTracker.Clear();
            throw new StorageException(e);
        }
    }
    
    public void Save(User user)
    {
        try
        {
            _context.Add(user);
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
}