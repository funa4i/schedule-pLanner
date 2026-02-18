using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Repository;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces;
using SchedulePlannerBack.Interfaces.Application;

namespace SchedulePlannerBack.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public UserService(IUserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    public User GetUserById(long userId)
    {
        _logger.LogInformation("Getting user by id: {UserId}" , userId);
        return _userRepository.GetById(userId) ?? throw new ElementNotFoundException(userId.ToString());
    }
}