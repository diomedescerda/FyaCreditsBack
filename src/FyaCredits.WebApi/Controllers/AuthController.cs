using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace FyaCredits.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("Auth")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("token")]
    public ActionResult<TokenResponse> CreateToken(TokenRequest request)
    {
        var expectedPassword = configuration["Authentication:DemoPassword"];
        if (string.IsNullOrWhiteSpace(request.CommercialName)
            || string.IsNullOrWhiteSpace(expectedPassword)
            || request.Password != expectedPassword)
            return Unauthorized();

        var key = configuration["Authentication:SigningKey"]
            ?? throw new InvalidOperationException("Authentication signing key is not configured.");
        var issuer = configuration["Authentication:Issuer"] ?? "fya-credits";
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.CommercialName.Trim()),
            new Claim(JwtRegisteredClaimNames.Sub, request.CommercialName.Trim())
        };
        var tokenLifetimeMinutes = configuration.GetValue("Authentication:TokenLifetimeMinutes", 60);
        var token = new JwtSecurityToken(
            issuer,
            issuer,
            claims,
            expires: DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes),
            signingCredentials: credentials);

        return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token)));
    }
}

public sealed record TokenRequest(string CommercialName, string Password);
public sealed record TokenResponse(string AccessToken);
