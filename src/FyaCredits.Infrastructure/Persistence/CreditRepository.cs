using FyaCredits.Application;
using FyaCredits.Domain;
using Microsoft.EntityFrameworkCore;

namespace FyaCredits.Infrastructure.Persistence;

public sealed class CreditRepository(ApplicationDbContext dbContext) : ICreditRepository
{
    public async Task AddAsync(Credit credit, CancellationToken cancellationToken)
    {
        dbContext.Credits.Add(credit);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CreditQueryResult> SearchAsync(
        CreditQuery query,
        CancellationToken cancellationToken)
    {
        var credits = dbContext.Credits.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.ClientName))
            credits = credits.Where(credit => EF.Functions.ILike(credit.ClientName, $"%{query.ClientName.Trim()}%"));
        if (!string.IsNullOrWhiteSpace(query.ClientId))
            credits = credits.Where(credit => EF.Functions.ILike(credit.ClientId, $"%{query.ClientId.Trim()}%"));
        if (!string.IsNullOrWhiteSpace(query.CommercialName))
            credits = credits.Where(credit => EF.Functions.ILike(credit.CommercialName, $"%{query.CommercialName.Trim()}%"));

        credits = query.SortBy.ToLowerInvariant() switch
        {
            "amount" => query.SortDirection == "asc"
                ? credits.OrderBy(credit => credit.Amount)
                : credits.OrderByDescending(credit => credit.Amount),
            _ => query.SortDirection == "asc"
                ? credits.OrderBy(credit => credit.RegisteredAtUtc)
                : credits.OrderByDescending(credit => credit.RegisteredAtUtc)
        };

        var totalCount = await credits.CountAsync(cancellationToken);
        var items = await credits
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new CreditQueryResult(items, totalCount);
    }
}
