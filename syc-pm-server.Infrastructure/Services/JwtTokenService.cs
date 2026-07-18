using Microsoft.IdentityModel.Tokens;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace syc_pm_server.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        public string CreateToken(User user)
        {
            var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_CHANGE_ME_NOW_WITH_ENOUGH_BITS") // TODO: muss noch in eine Konfigurationsdatei ausgelagert werden
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "syc-pm",
                audience: "syc-pm",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
