using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases
{
    public class GetUserUseCase
    {
        public User Execute()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Username = "test-user"
            };
        }
    }
}
