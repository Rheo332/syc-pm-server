using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.UseCases;

namespace syc_pm_server.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetUserUseCase _getUserUseCase;
    private readonly CreateUserUseCase _createUserUseCase;

    public UserController(GetUserUseCase getUserUseCase, CreateUserUseCase createUserUseCase)
    {
        _getUserUseCase = getUserUseCase;
        _createUserUseCase = createUserUseCase;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> Get(string username)
    {
        var user = await _getUserUseCase.Execute(username);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] syc_pm_server.Application.DTO.CreateUserRequest request)
    {
        var success = await _createUserUseCase.Execute(request);
        if (!success)
            return Conflict(new { Message = "Benutzername existiert bereits" });

        return Ok(new { Message = "Benutzer erfolgreich erstellt" });
    }
}