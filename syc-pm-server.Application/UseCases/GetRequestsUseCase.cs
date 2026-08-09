using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases
{
    public class GetRequestsUseCase
    {
        private readonly IRequestRepository _requestRepository;

        public GetRequestsUseCase(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<List<RequestResponseDto>> ExecuteAsync(Guid userId)
        {
            var reqs = await _requestRepository.GetAllAsync(userId);
            return reqs.Select(r => new RequestResponseDto
            {
                Id = r.Id,
                Type = r.Type,
                Username = r.Username,
                Payload = r.Payload,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}