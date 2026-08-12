using FyaCredits.Domain;

namespace FyaCredits.Application;

public sealed class CreditService(
    ICreditRepository repository,
    INotificationQueue notificationQueue,
    ICurrentUser currentUser,
    TimeProvider timeProvider)
{
    public async Task<CreditResponse> RegisterAsync(
        RegisterCreditCommand command,
        CancellationToken cancellationToken)
    {
        var credit = Credit.Create(
            command.ClientName,
            command.ClientId,
            command.Amount,
            command.InterestRate,
            command.TermMonths,
            currentUser.CommercialName,
            timeProvider.GetUtcNow());

        await repository.AddAsync(credit, cancellationToken);
        await notificationQueue.EnqueueAsync(
            new CreditRegisteredNotification(
                credit.Id,
                credit.ClientName,
                credit.ClientId,
                credit.Amount,
                credit.InterestRate,
                credit.TermMonths,
                credit.CommercialName,
                credit.RegisteredAtUtc),
            cancellationToken);

        return ToResponse(credit);
    }

    public async Task<PagedResult<CreditResponse>> SearchAsync(
        CreditQuery query,
        CancellationToken cancellationToken)
    {
        var result = await repository.SearchAsync(query, cancellationToken);

        return new PagedResult<CreditResponse>(
            result.Items.Select(ToResponse).ToList(),
            query.Page,
            query.PageSize,
            result.TotalCount);
    }

    private static CreditResponse ToResponse(Credit credit) => new(
        credit.Id,
        credit.ClientName,
        credit.ClientId,
        credit.Amount,
        credit.InterestRate,
        credit.TermMonths,
        credit.CommercialName,
        credit.RegisteredAtUtc);
}
