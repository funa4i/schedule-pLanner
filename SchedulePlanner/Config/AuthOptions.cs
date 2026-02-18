using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SchedulePlannerBack;

public class AuthOptions
{
    public string Issuer {get; set;} 
    public string Audience {get; set;} 
    public  string Key {get; set;} 
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key) => new(Encoding.UTF8.GetBytes(key));
}