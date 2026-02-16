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
    private readonly JwtService _jwtService;
    
    public AuthenticationService(UserRepository userRepository, ILogger logger, JwtService jwtService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtService = jwtService;
    }

    public void RegisterUser(User user)
    {
        _logger.LogInformation("New user: {UserName}", user.Login);
        var passwordHasher = new PasswordHasher<User>();
        user.Password = passwordHasher.HashPassword(user, user.Password);
        _userRepository.Save(user);
    }
    
    public string ValidateUser(User user)
    {
        _logger.LogInformation("Validating user: {UserName}", user.Login);
        var passwordHasher = new PasswordHasher<User>();
        var dbUser = _userRepository.GetByName(user.Login) 
                     ?? throw new ElementNotFoundException(user.Login);
        var result = passwordHasher.VerifyHashedPassword(dbUser, dbUser.Password, user.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException();
        }
        return _jwtService.GetToken(user);
    }
}