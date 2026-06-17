using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.DTO;
using syc_pm_server.Application.UseCases;

namespace syc_pm_server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUserUseCase _loginUser;

    public AuthController(LoginUserUseCase loginUser)
    {
        _loginUser = loginUser;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _loginUser.Execute(request);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
}