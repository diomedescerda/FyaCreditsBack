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

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            credits = long.TryParse(search, out _)
                ? credits.Where(credit => EF.Functions.ILike(credit.ClientId, $"%{search}%"))
                : credits.Where(credit =>
                    EF.Functions.ILike(credit.ClientName, $"%{search}%") ||
                    EF.Functions.ILike(credit.CommercialName, $"%{search}%"));
        }

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
