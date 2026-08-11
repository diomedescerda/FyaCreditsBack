namespace FyaCredits.Domain;

public sealed class Credit
{
    private Credit()
    {
    }

    private Credit(
        string clientName,
        string clientId,
        long amount,
        decimal interestRate,
        int termMonths,
        string commercialName,
        DateTimeOffset registeredAtUtc)
    {
        Id = Guid.NewGuid();
        ClientName = clientName;
        ClientId = clientId;
        Amount = amount;
        InterestRate = interestRate;
        TermMonths = termMonths;
        CommercialName = commercialName;
        RegisteredAtUtc = registeredAtUtc;
    }

    public Guid Id { get; private set; }
    public string ClientName { get; private set; } = string.Empty;
    public string ClientId { get; private set; } = string.Empty;
    public long Amount { get; private set; }
    public decimal InterestRate { get; private set; }
    public int TermMonths { get; private set; }
    public string CommercialName { get; private set; } = string.Empty;
    public DateTimeOffset RegisteredAtUtc { get; private set; }

    public static Credit Create(
        string clientName,
        string clientId,
        long amount,
        decimal interestRate,
        int termMonths,
        string commercialName,
        DateTimeOffset registeredAtUtc)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            throw new ArgumentException("Client name is required.", nameof(clientName));
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID is required.", nameof(clientId));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (interestRate < 0)
            throw new ArgumentOutOfRangeException(nameof(interestRate), "Interest rate cannot be negative.");
        if (termMonths <= 0)
            throw new ArgumentOutOfRangeException(nameof(termMonths), "Term must be greater than zero.");
        if (string.IsNullOrWhiteSpace(commercialName))
            throw new ArgumentException("Commercial representative is required.", nameof(commercialName));

        return new Credit(
            clientName.Trim(),
            clientId.Trim(),
            amount,
            interestRate,
            termMonths,
            commercialName.Trim(),
            registeredAtUtc);
    }
}
