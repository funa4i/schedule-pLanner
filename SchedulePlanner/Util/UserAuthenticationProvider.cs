using System.Security.Claims;
using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Util;

public class UserAuthenticationProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAuthenticationProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserAuthenticationInformation GetUserInformation()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        long.TryParse(user?.Claims
            .FirstOrDefault(c => c.Type == "UserId")?.Value,
            out long userId);
        return new UserAuthenticationInformation()
        {
            UserId = userId == 0? null: userId,
        };
    }
}