using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Interfaces;

public interface IUserRepository
{
    User? GetByName(string name);
    User? GetById(long id);
    void Save(User user);
}