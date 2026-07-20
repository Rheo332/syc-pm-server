namespace syc_pm_server.Application.DTO
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string PublicKey { get; set; } = null!;
        public string EncryptedPrivateKey { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
}
