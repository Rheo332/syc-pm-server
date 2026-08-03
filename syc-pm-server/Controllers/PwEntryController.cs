using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.DTO;
using syc_pm_server.Application.UseCases;
using System.Security.Claims;

namespace syc_pm_server.Controllers;

[ApiController]
[Route("api/entries")]
public class PwEntryController : ControllerBase
{
    private readonly GetPwEntryUseCase _getPwEntryUseCase;
    private readonly CreatePwEntryUseCase _createPwEntryUseCase;

    public PwEntryController(GetPwEntryUseCase getPwEntryUseCase, CreatePwEntryUseCase createPwEntryUseCase)
    {
        _getPwEntryUseCase = getPwEntryUseCase;
        _createPwEntryUseCase = createPwEntryUseCase;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetEntries()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _getPwEntryUseCase.Execute(userId);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateEntry([FromBody] CreatePwEntryRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _createPwEntryUseCase.Execute(userId, request);
        if (response)
        {
            return Ok(response);
        }
        return BadRequest();
    }
}

