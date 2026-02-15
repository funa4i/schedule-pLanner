using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Repository;
using SchedulePlannerBack.Exceptions;

namespace SchedulePlannerBack.Service;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly ILogger _logger;

    public UserService(UserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    private User GetUserById(long userId)
    {
        _logger.LogInformation("Getting user by id: {UserId}" , userId);
        return _userRepository.GetById(userId) ?? throw new ElementNotFoundException(userId.ToString());
    }
}