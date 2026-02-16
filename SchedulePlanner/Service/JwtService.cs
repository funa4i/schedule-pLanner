using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SchedulePlannerBack.Config;
using SchedulePlannerBack.Domain.Entity;

namespace SchedulePlannerBack.Service;

public class JwtService
{
    private readonly AuthOptionsConfigurer _authOptionsConfigurer;
    public string GetToken(User user)
    {
        var authOptions = _authOptionsConfigurer.AuthOptions;
        var claim = new[]
        {
            new Claim("UserId", user.Id.ToString())
        };
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: authOptions.ISSUER,
            audience: authOptions.AUDIENCE,
            claims: claim ,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(20)),
            signingCredentials: new
                SigningCredentials(authOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)));
    }
}