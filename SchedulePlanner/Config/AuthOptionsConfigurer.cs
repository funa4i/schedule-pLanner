namespace SchedulePlannerBack.Config;

public class AuthOptionsConfigurer(IConfiguration config)
{
    private readonly Lazy<AuthOptions> _authOptions = new(() =>
    {
        return config.GetSection("AuthOptions").Get<AuthOptions>() ??
               throw new InvalidDataException(nameof(AuthOptions));
    });
    public AuthOptions AuthOptions => _authOptions.Value;
}