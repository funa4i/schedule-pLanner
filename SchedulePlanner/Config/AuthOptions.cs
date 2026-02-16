using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SchedulePlannerBack;

public class AuthOptions(IConfiguration config)
{
    public string ISSUER {get; set;} 
    public string AUDIENCE {get; set;} 
    public  string KEY {get; set;} 
    public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));
}