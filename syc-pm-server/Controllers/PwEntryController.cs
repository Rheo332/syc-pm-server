using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.UseCases;
using System.Security.Claims;

namespace syc_pm_server.Controllers;

[ApiController]
[Route("api/entries")]
public class PwEntryController : ControllerBase
{
    private readonly GetPwEntryUseCase _getPwEntryUseCase;

    public PwEntryController(GetPwEntryUseCase getPwEntryUseCase)
    {
        _getPwEntryUseCase = getPwEntryUseCase;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetEntries()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _getPwEntryUseCase.Execute(userId);
        return Ok(response);
    }
}

