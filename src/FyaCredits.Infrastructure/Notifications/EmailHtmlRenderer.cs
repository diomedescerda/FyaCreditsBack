using System.Net.Http.Json;
using FyaCredits.Application;
using Microsoft.Extensions.Configuration;

namespace FyaCredits.Infrastructure.Notifications;

public interface IEmailTemplateRenderer
{
    Task<string?> RenderCreditAsync(CreditRegisteredNotification notification, CancellationToken cancellationToken);
    Task<string?> RenderPasswordResetAsync(string email, string resetUrl, CancellationToken cancellationToken);
}

public sealed class EmailHtmlRenderer(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : IEmailTemplateRenderer
{
    public async Task<string?> RenderCreditAsync(
        CreditRegisteredNotification notification,
        CancellationToken cancellationToken)
    {
        var response = await PostAsync(new
        {
            template = "credit",
            notification.ClientName,
            notification.ClientId,
            notification.Amount,
            notification.InterestRate,
            notification.TermMonths,
            notification.CommercialName,
            notification.RegisteredAtUtc
        }, cancellationToken);

        return await ReadBodyAsync(response, cancellationToken);
    }

    public async Task<string?> RenderPasswordResetAsync(
        string email,
        string resetUrl,
        CancellationToken cancellationToken)
    {
        var response = await PostAsync(new { template = "reset-password", email, resetUrl }, cancellationToken);

        return await ReadBodyAsync(response, cancellationToken);
    }

    private async Task<HttpResponseMessage?> PostAsync(object payload, CancellationToken cancellationToken)
    {
        var url = configuration["Email:Renderer:Url"];
        if (string.IsNullOrWhiteSpace(url))
            return null;

        try
        {
            using var client = httpClientFactory.CreateClient("EmailRenderer");
            return await client.PostAsJsonAsync(url, payload, cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static async Task<string?> ReadBodyAsync(HttpResponseMessage? response, CancellationToken cancellationToken)
    {
        if (response is null || !response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
