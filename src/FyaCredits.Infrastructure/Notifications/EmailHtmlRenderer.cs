using System.Net.Http.Json;
using FyaCredits.Application;
using Microsoft.Extensions.Configuration;

namespace FyaCredits.Infrastructure.Notifications;

public interface IEmailTemplateRenderer
{
    Task<string?> RenderAsync(CreditRegisteredNotification notification, CancellationToken cancellationToken);
}

public sealed class EmailHtmlRenderer(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : IEmailTemplateRenderer
{
    public async Task<string?> RenderAsync(
        CreditRegisteredNotification notification,
        CancellationToken cancellationToken)
    {
        var url = configuration["Email:Renderer:Url"];
        if (string.IsNullOrWhiteSpace(url))
            return null;

        try
        {
            using var client = httpClientFactory.CreateClient("EmailRenderer");
            var response = await client.PostAsJsonAsync(url, notification, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
