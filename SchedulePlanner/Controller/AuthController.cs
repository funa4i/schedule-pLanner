using Microsoft.AspNetCore.Mvc;
using SchedulePlannerBack.Adapters;
using SchedulePlannerBack.Domain.Bindings;

namespace SchedulePlannerBack.Controller;

[ApiController]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationAdapter _authenticationAdapter;

    public AuthController(AuthenticationAdapter authenticationAdapter)
    {
        _authenticationAdapter = authenticationAdapter;
    }

    [HttpPost]
    public IActionResult SignUp([FromBody] UserRequest userRequest)
    {
        return _authenticationAdapter.CreateUser(userRequest).GetResponse(Request, Response);
    }

    [HttpPost]
    public IActionResult SignIn([FromBody] UserRequest userRequest)
    {
        return _authenticationAdapter.ValidateUser(userRequest).GetResponse(Request, Response);
    }
}