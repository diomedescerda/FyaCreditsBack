using System.ComponentModel.DataAnnotations;
using System.Net;
using FyaCredits.Application;
using FyaCredits.Infrastructure.Persistence;
using FyaCredits.WebApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FyaCredits.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("Auth")]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IEmailSender emailSender,
    IConfiguration configuration,
    JwtTokenService tokenService) : ControllerBase
{
    private const string CommercialRole = "Comercial";

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            FullName = request.FullName.Trim(),
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return ValidationProblem(result);

        if (!await roleManager.RoleExistsAsync(CommercialRole))
            await roleManager.CreateAsync(new IdentityRole<Guid>(CommercialRole));
        await userManager.AddToRoleAsync(user, CommercialRole);

        return Ok(new { message = "Usuario registrado correctamente." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return Unauthorized();

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
            return Problem(
                "Demasiados intentos fallidos. Cuenta bloqueada temporalmente, intenta en unos minutos.",
                statusCode: StatusCodes.Status423Locked);
        if (!result.Succeeded)
            return Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateToken(user, roles);

        return Ok(new TokenResponse(token, user.FullName, user.Email ?? string.Empty));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return Ok();

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLinkBase = configuration["Authentication:ResetLinkBaseUrl"]
            ?? "http://localhost:8080";
        var resetUrl = $"{resetLinkBase}/api/auth/reset-link" +
                       $"?token={Uri.EscapeDataString(token)}" +
                       $"&email={Uri.EscapeDataString(request.Email.Trim())}";

        await emailSender.SendPasswordResetAsync(request.Email.Trim(), resetUrl, cancellationToken);

        return Ok();
    }

    [HttpGet("reset-link")]
    public IActionResult ResetLink([FromQuery] string token, [FromQuery] string email)
    {
        var deepBase = configuration["Authentication:ResetPasswordBaseUrl"]
            ?? "com.fya.credits://reset-password";
        var webBase = configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";

        var deepUrl = $"{deepBase}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
        var webUrl = $"{webBase}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        var html = $$"""
            <!doctype html>
            <html lang="es">
            <head>
              <meta charset="utf-8" />
              <meta name="viewport" content="width=device-width, initial-scale=1" />
              <title>Restablece tu contraseña</title>
              <style>
                body { margin: 0; padding: 0; min-height: 100vh; display: flex; align-items: center; justify-content: center;
                  background: linear-gradient(126deg, #052224 0%, #073b39 48%, #00a968 100%);
                  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; }
                .card { background: #fff; max-width: 440px; width: calc(100% - 32px); border-radius: 18px; padding: 40px 32px; text-align: center; }
                .eyebrow { color: #6b7280; font-size: 12px; letter-spacing: 2px; text-transform: uppercase; margin: 0 0 8px; }
                h1 { color: #052224; font-size: 24px; margin: 0 0 12px; }
                p { color: #6b7280; font-size: 14px; line-height: 1.6; margin: 0 0 24px; }
                .btn { display: block; background: #00d280; color: #052224; font-weight: 800; text-decoration: none;
                  padding: 14px; border-radius: 10px; margin-bottom: 12px; }
                .link { color: #6b7280; font-size: 13px; }
              </style>
            </head>
            <body>
              <div class="card">
                <p class="eyebrow">Fya Social Capital</p>
                <h1>Restablece tu contraseña</h1>
                <p>Selecciona cómo quieres continuar para elegir una nueva contraseña.</p>
                <a class="btn" href="{{WebUtility.HtmlEncode(deepUrl)}}">Abrir en la app de Fya</a>
                <a class="link" href="{{WebUtility.HtmlEncode(webUrl)}}">Continuar en el navegador</a>
              </div>
            </body>
            </html>
            """;

        return Content(html, "text/html; charset=utf-8");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "No se pudo restablecer la contraseña."
            });

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (!result.Succeeded)
            return ValidationProblem(result);

        await userManager.ResetAccessFailedCountAsync(user);
        return Ok();
    }

    private IActionResult ValidationProblem(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
        return ValidationProblem(ModelState);
    }
}

public sealed class RegisterRequest
{
    [Required, MaxLength(150)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string Password { get; init; } = string.Empty;
}

public sealed class ForgotPasswordRequest
{
    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; init; } = string.Empty;
}

public sealed class ResetPasswordRequest
{
    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Token { get; init; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Password { get; init; } = string.Empty;
}

public sealed record TokenResponse(string AccessToken, string FullName, string Email);
