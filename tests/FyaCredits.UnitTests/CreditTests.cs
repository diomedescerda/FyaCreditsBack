using FyaCredits.Domain;

namespace FyaCredits.UnitTests;

public sealed class CreditTests
{
    [Fact]
    public void Create_StoresMoneyAsAnInteger()
    {
        var credit = Credit.Create(
            "Pepito Perez",
            "123",
            7800000,
            2,
            10,
            "Ana Comercial",
            DateTimeOffset.UtcNow);

        Assert.Equal(7800000, credit.Amount);
    }

    [Fact]
    public void Create_RejectsNonPositiveAmount()
    {
        var action = () => Credit.Create(
            "Pepito Perez",
            "123",
            0,
            2,
            10,
            "Ana Comercial",
            DateTimeOffset.UtcNow);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Create_RequiresCommercialName()
    {
        var action = () => Credit.Create(
            "Pepito Perez",
            "123",
            7800000,
            2,
            10,
            "",
            DateTimeOffset.UtcNow);

        Assert.Throws<ArgumentException>(action);
    }
}
