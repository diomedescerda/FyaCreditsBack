using System.ComponentModel.DataAnnotations;

namespace FyaCredits.WebApi.Models;

public sealed class RegisterCreditRequest
{
    [Required, MaxLength(150)]
    public string ClientName { get; init; } = string.Empty;

    [Required, MaxLength(50)]
    public string ClientId { get; init; } = string.Empty;

    [Range(1, long.MaxValue)]
    public long Amount { get; init; }

    [Range(0, 100)]
    public decimal InterestRate { get; init; }

    [Range(1, 600)]
    public int TermMonths { get; init; }
}

public sealed class CreditQueryRequest
{
    public string? ClientName { get; init; }
    public string? ClientId { get; init; }
    public string? CommercialName { get; init; }
    public string SortBy { get; init; } = "date";
    public string SortDirection { get; init; } = "desc";

    [Range(1, 100000)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;
}
