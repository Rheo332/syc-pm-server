using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.UseCases;

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

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pwEntries = await _getPwEntryUseCase.Execute();
        return Ok(pwEntries);
    }
}

