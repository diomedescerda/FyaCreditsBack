using System.ComponentModel.DataAnnotations;
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
        var frontendBase = configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";
        var resetUrl = $"{frontendBase}/reset-password" +
                       $"?token={Uri.EscapeDataString(token)}" +
                       $"&email={Uri.EscapeDataString(request.Email.Trim())}";

        await emailSender.SendPasswordResetAsync(request.Email.Trim(), resetUrl, cancellationToken);

        return Ok();
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
