using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Interfaces.Application;

public interface IUserService
{
    User GetUserById(long userId);
}