using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FyaCredits.UnitTests;

public sealed class CreditApiBehaviorTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public CreditApiBehaviorTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Register_WithoutToken_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/credits", new
        {
            clientName = "Pepito Perez",
            clientId = "123",
            amount = 7800000L,
            interestRate = 2m,
            termMonths = 10
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithBlankClientName_ReturnsBadRequest()
    {
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var response = await client.PostAsJsonAsync("/api/credits", new
        {
            clientName = "   ",
            clientId = "123",
            amount = 7800000L,
            interestRate = 2m,
            termMonths = 10
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidAmount_ReturnsBadRequest()
    {
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var response = await client.PostAsJsonAsync("/api/credits", new
        {
            clientName = "Pepito Perez",
            clientId = "123",
            amount = 0,
            interestRate = 2m,
            termMonths = 10
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Query_WithInvalidSortBy_ReturnsBadRequest()
    {
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var response = await client.GetAsync("/api/credits?sortBy=client");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task AuthenticateAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/token", new
        {
            commercialName = "Ana Comercial",
            password = "development-password"
        });
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token!.AccessToken);
    }

    private sealed record TokenResponse(string AccessToken);
}
