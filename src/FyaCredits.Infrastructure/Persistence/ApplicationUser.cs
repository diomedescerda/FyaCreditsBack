using Microsoft.AspNetCore.Identity;

namespace FyaCredits.Infrastructure.Persistence;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
}
