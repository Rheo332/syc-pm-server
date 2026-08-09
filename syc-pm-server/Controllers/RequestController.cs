using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syc_pm_server.Application.DTO;
using syc_pm_server.Application.UseCases;
using System.Security.Claims;

namespace syc_pm_server.Controllers
{
    [ApiController]
    [Route("api/requests")]
    public class RequestController : ControllerBase
    {
        private readonly CreateRequestUseCase _createRequestUseCase;
        private readonly GetRequestsUseCase _getRequestsUseCase;
        private readonly DeleteRequestUseCase _deleteRequestUseCase;

        public RequestController(
            CreateRequestUseCase createRequestUseCase,
            GetRequestsUseCase getRequestsUseCase,
            DeleteRequestUseCase deleteRequestUseCase)
        {
            _createRequestUseCase = createRequestUseCase;
            _getRequestsUseCase = getRequestsUseCase;
            _deleteRequestUseCase = deleteRequestUseCase;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestDto request)
        {
            var response = await _createRequestUseCase.ExecuteAsync(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var requests = await _getRequestsUseCase.ExecuteAsync(userId);
            return Ok(requests);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var success = await _deleteRequestUseCase.ExecuteAsync(userId, id);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}