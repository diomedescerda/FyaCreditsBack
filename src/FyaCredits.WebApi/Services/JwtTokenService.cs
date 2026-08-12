using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FyaCredits.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;

namespace FyaCredits.WebApi.Services;

public sealed class JwtTokenService(IConfiguration configuration)
{
    public string CreateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var key = configuration["Authentication:SigningKey"]
            ?? throw new InvalidOperationException("Authentication signing key is not configured.");
        var issuer = configuration["Authentication:Issuer"] ?? "fya-credits";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var tokenLifetimeMinutes = configuration.GetValue("Authentication:TokenLifetimeMinutes", 60);
        var token = new JwtSecurityToken(
            issuer,
            issuer,
            claims,
            expires: DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
