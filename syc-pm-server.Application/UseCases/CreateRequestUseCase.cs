using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases
{
    public class CreateRequestUseCase
    {
        private readonly IRequestRepository _requestRepository;

        public CreateRequestUseCase(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<RequestResponseDto> ExecuteAsync(RequestDto dto)
        {
            var req = new Request
            {
                Id = Guid.NewGuid(),
                Type = dto.Type,
                Username = dto.Username,
                Payload = dto.Payload,
                CreatedAt = DateTime.UtcNow
            };

            await _requestRepository.CreateAsync(req);

            return new RequestResponseDto
            {
                Id = req.Id,
                Type = req.Type,
                Username = req.Username,
                Payload = req.Payload,
                CreatedAt = req.CreatedAt
            };
        }
    }
}