using Microsoft.AspNetCore.Mvc;
using SB.PublicInstitutions.Services.Abstractions;
using Shared.DTOs;

[ApiController]
[Route("[controller]")]
public class UserController(IUsersService userService) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<RegisterUserResponse>> Register([FromBody] User request)
    {
        var newUser = await userService.Register(request);
        return Ok(newUser);
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginUserResponse>> Login([FromBody] User request)
    {
        var token = await userService.Login(request);
        return Ok(new { Token = token });
    }
}