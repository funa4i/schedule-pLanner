using Microsoft.AspNetCore.Mvc;
using SchedulePlannerBack.Adapters;
using SchedulePlannerBack.Domain.Bindings;

namespace SchedulePlannerBack.Controller;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationAdapter _authenticationAdapter;

    public AuthController(AuthenticationAdapter authenticationAdapter)
    {
        _authenticationAdapter = authenticationAdapter;
    }

    [HttpPost("signup")]
    public IActionResult SignUp([FromBody] UserRequest userRequest)
    {
        return _authenticationAdapter.CreateUser(userRequest).GetResponse(Request, Response);
    }

    [HttpGet("signin")]
    public IActionResult SignIn([FromBody] UserRequest userRequest)
    {
        return _authenticationAdapter.ValidateUser(userRequest).GetResponse(Request, Response);
    }
}