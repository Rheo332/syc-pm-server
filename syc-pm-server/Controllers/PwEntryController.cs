using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.DTO;
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

    [HttpPost]
    public async Task<IActionResult> Get([FromBody] PwEntryRequest request)
    {
        var response = await _getPwEntryUseCase.Execute(request);
        return Ok(response);
    }
}

