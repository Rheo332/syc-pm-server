using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases
{
    public class DeleteRequestUseCase
    {
        private readonly IRequestRepository _requestRepository;

        public DeleteRequestUseCase(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<bool> ExecuteAsync(Guid userId, Guid id)
        {
            return await _requestRepository.DeleteAsync(userId, id);
        }
    }
}