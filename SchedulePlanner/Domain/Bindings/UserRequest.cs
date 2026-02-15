using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Util;

namespace SchedulePlannerBack.Domain.Bindings;

public class UserRequest : IValidation
{
    public UserRequest(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login {get; set;}
    public string Password {get; set;}

    public void Validate()
    {
        if (string.IsNullOrEmpty(Login))
            throw new ValidationException("Login is null or empty");
        if (string.IsNullOrEmpty(Password))
            throw new ValidationException("Password is null or empty");
    }
}