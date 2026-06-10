using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.UseCases;

namespace syc_pm_server.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetUserUseCase _getUserUseCase;

    public UserController(GetUserUseCase getUserUseCase)
    {
        _getUserUseCase = getUserUseCase;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> Get(string username)
    {
        var user = await _getUserUseCase.Execute(username);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
}