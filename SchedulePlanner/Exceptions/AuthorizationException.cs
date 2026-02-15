namespace SchedulePlannerBack.Exceptions;

public class AuthorizationException(string message) : Exception($"Authorization exception: {message}")
{
    public string ExceptionMessage {get; private set;} = message;
}