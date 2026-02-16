using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulePlannerBack.Adapters;

namespace SchedulePlannerBack.Controller;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly UserAdapter _userAdapter;


    [HttpGet]
    [Authorize]
    public IActionResult GetUserProfile()
    {
       return _userAdapter.GetUserProfileWithAuthentication().GetResponse(Request, Response);
    }
}