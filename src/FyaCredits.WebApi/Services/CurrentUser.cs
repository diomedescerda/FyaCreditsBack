using System.Security.Claims;
using FyaCredits.Application;

namespace FyaCredits.WebApi.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string CommercialName =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
        ?? throw new InvalidOperationException("Authenticated commercial identity is missing.");
}
