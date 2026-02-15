using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Domain.Repository;
using SchedulePlannerBack.Exceptions;

namespace SchedulePlannerBack.Service;

public class AuthenticationService
{
    private readonly UserRepository _userRepository;
    private readonly ILogger _logger;
    
    public AuthenticationService(UserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    private void RegisterUser(User user)
    {
        _logger.LogInformation("New user: {UserName}", user.Name);
        var passwordHasher = new PasswordHasher<User>();
        user.Password = passwordHasher.HashPassword(user, user.Password);
        _userRepository.Save(user);
    }
    
    private void ValidateUser(User user)
    {
        _logger.LogInformation("Validating user: {UserName}", user.Name);
        var passwordHasher = new PasswordHasher<User>();
        var dbUser = _userRepository.GetByName(user.Name) 
                     ?? throw new ElementNotFoundException(user.Name);
        var result = passwordHasher.VerifyHashedPassword(dbUser, dbUser.Password, user.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException();
        }
    }
}