using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FyaCredits.UnitTests;

public sealed class CreditApiBehaviorTests : IClassFixture<CreditApiBehaviorTests.ApiFactory>
{
    private const string TestEmail = "test.comercial@fya.local";
    private const string TestPassword = "TestPass123!";

    private readonly WebApplicationFactory<Program> factory;

    public CreditApiBehaviorTests(ApiFactory factory)
    {
        this.factory = factory;
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("ConnectionStrings:DefaultConnection",
                "Host=localhost;Port=5432;Database=fyacredits;Username=postgres;Password=change-me");
            builder.UseSetting("Authentication:SeedEmail", TestEmail);
            builder.UseSetting("Authentication:SeedPassword", TestPassword);
            builder.UseSetting("Frontend:BaseUrl", "http://localhost:4200");
        }
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

    private async Task AuthenticateAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = TestEmail,
            password = TestPassword
        });
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token!.AccessToken);
    }

    private sealed record TokenResponse(string AccessToken);
}
