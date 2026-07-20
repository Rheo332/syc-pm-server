using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases
{
    public class PreloginUseCase
    {
        private readonly IUserRepository _userRepository;

        public PreloginUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PreloginResponse> Execute(PreloginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user is null)
                return new PreloginResponse { Pbkdf2Salt = "", PasswordSalt = "" };

            return new PreloginResponse
            {
                Pbkdf2Salt = user.Pbkdf2Salt,
                PasswordSalt = user.PasswordSalt
            };
        }
    }
}