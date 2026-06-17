using Microsoft.AspNetCore.Mvc;

namespace syc_pm_server.Controllers;

[ApiController]
[Route("api/vaults")]
public class VaultController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("vaults get erfolgreich");
    }
}

