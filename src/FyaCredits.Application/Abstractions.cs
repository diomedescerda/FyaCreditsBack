using FyaCredits.Domain;

namespace FyaCredits.Application;

public interface ICreditRepository
{
    Task AddAsync(Credit credit, CancellationToken cancellationToken);
    Task<CreditQueryResult> SearchAsync(CreditQuery query, CancellationToken cancellationToken);
}

public interface INotificationQueue
{
    ValueTask EnqueueAsync(CreditRegisteredNotification notification, CancellationToken cancellationToken);
}

public interface ICurrentUser
{
    string CommercialName { get; }
}

public interface IEmailSender
{
    Task SendCreditRegisteredAsync(CreditRegisteredNotification notification, CancellationToken cancellationToken);
}

public sealed record CreditRegisteredNotification(
    Guid CreditId,
    string ClientName,
    string ClientId,
    long Amount,
    decimal InterestRate,
    int TermMonths,
    string CommercialName,
    DateTimeOffset RegisteredAtUtc);

public sealed record RegisterCreditCommand(
    string ClientName,
    string ClientId,
    long Amount,
    decimal InterestRate,
    int TermMonths);

public sealed record CreditQuery(
    string? ClientName,
    string? ClientId,
    string? CommercialName,
    string SortBy,
    string SortDirection,
    int Page,
    int PageSize);

public sealed record CreditQueryResult(
    IReadOnlyList<Credit> Items,
    int TotalCount);

public sealed record CreditResponse(
    Guid Id,
    string ClientName,
    string ClientId,
    long Amount,
    decimal InterestRate,
    int TermMonths,
    string CommercialName,
    DateTimeOffset RegisteredAtUtc);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount);
