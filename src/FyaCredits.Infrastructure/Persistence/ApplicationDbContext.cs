using FyaCredits.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FyaCredits.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Credit> Credits => Set<Credit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.ToTable("credits");
            entity.HasKey(credit => credit.Id);
            entity.Property(credit => credit.ClientName).HasMaxLength(150).IsRequired();
            entity.Property(credit => credit.ClientId).HasMaxLength(50).IsRequired();
            entity.Property(credit => credit.Amount).HasColumnType("bigint").IsRequired();
            entity.Property(credit => credit.InterestRate).HasPrecision(5, 2).IsRequired();
            entity.Property(credit => credit.TermMonths).IsRequired();
            entity.Property(credit => credit.CommercialName).HasMaxLength(150).IsRequired();
            entity.Property(credit => credit.RegisteredAtUtc).IsRequired();
            entity.HasIndex(credit => credit.ClientId);
            entity.HasIndex(credit => credit.RegisteredAtUtc);
            entity.HasIndex(credit => credit.Amount);
            entity.HasIndex(credit => credit.CommercialName);
        });
    }
}
