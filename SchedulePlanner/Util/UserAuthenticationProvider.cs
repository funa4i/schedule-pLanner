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
        var value = user?.Claims
            .FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (value == null)
        {
            return new UserAuthenticationInformation()
            {
                UserId =  null,
            };
        }
        return new UserAuthenticationInformation()
        {
            UserId =  long.Parse(value),
        };
    }
}