using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.UseCases;
using System.Security.Claims;

namespace syc_pm_server.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetUserUseCase _getUserUseCase;
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly DeleteUserUseCase _deleteUserUseCase;
    private readonly GetAllUsersUseCase _getAllUsersUseCase;
    private readonly GetUserAccessUseCase _getUserAccessUseCase;

    public UserController(
        GetUserUseCase getUserUseCase,
        CreateUserUseCase createUserUseCase,
        DeleteUserUseCase deleteUserUseCase,
        GetAllUsersUseCase getAllUsersUseCase,
        GetUserAccessUseCase getUserAccessUseCase)
    {
        _getUserUseCase = getUserUseCase;
        _createUserUseCase = createUserUseCase;
        _deleteUserUseCase = deleteUserUseCase;
        _getAllUsersUseCase = getAllUsersUseCase;
        _getUserAccessUseCase = getUserAccessUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _getAllUsersUseCase.Execute();
        return Ok(users.Select(u => new
        {
            u.Id,
            u.Username,
            u.PublicKey
        }));
    }

    [HttpGet("{userId:guid}/access")]
    public async Task<IActionResult> GetUserAccess(Guid userId)
    {
        var accessList = await _getUserAccessUseCase.Execute(userId);
        return Ok(accessList);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> Get(string username)
    {
        var user = await _getUserUseCase.Execute(username);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("admin/publickey")]
    public async Task<IActionResult> GetAdminPublicKey()
    {
        var admin = await _getUserUseCase.Execute("admin");
        if (admin == null) return NotFound("Admin user not found");

        return Ok(new { publicKey = admin.PublicKey });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Application.DTO.CreateUserRequest request)
    {
        var success = await _createUserUseCase.Execute(request);
        if (!success)
            return Conflict(new { Message = "Username already exists" });

        return Ok(new { Message = "User created successfully" });
    }

    [Authorize]
    [HttpDelete("{username}")]
    public async Task<IActionResult> Delete(string username)
    {
        var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _deleteUserUseCase.Execute(adminId, username);
        if (!success)
            return NotFound(new { Message = "User not found" });

        return Ok(new { Message = "User deleted successfully" });
    }
}