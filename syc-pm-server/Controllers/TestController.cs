using Microsoft.AspNetCore.Mvc;

namespace syc_pm_server.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API Test erfolgreich");
        }
    }
}
