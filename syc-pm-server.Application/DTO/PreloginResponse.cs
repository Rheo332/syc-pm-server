namespace syc_pm_server.Application.DTO
{
    public class PreloginResponse
    {
        public string Pbkdf2Salt { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
    }
}
