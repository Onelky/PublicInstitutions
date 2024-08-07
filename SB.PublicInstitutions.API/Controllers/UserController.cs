using Microsoft.AspNetCore.Mvc;
using SB.PublicInstitutions.Services.Abstractions;

[ApiController]
[Route("[controller]")]
public class UserController(IUsersService userService) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] User request)
    {
        var newUser = await userService.Register(request);
        return Ok(newUser);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] User request)
    {
        var token = await userService.Login(request);
        return Ok(new { Token = token });
    }
}