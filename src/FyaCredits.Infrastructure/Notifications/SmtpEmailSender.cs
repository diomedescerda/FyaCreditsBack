using System.Net;
using System.Net.Mail;
using FyaCredits.Application;
using Microsoft.Extensions.Configuration;

namespace FyaCredits.Infrastructure.Notifications;

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendCreditRegisteredAsync(
        CreditRegisteredNotification notification,
        CancellationToken cancellationToken)
    {
        var host = configuration["Email:Smtp:Host"];
        var from = configuration["Email:From"];
        var recipient = configuration["Email:To"];
        var username = configuration["Email:Smtp:Username"];
        var password = configuration["Email:Smtp:Password"];

        if (string.IsNullOrWhiteSpace(host)
            || string.IsNullOrWhiteSpace(from)
            || string.IsNullOrWhiteSpace(recipient))
            throw new InvalidOperationException("Email SMTP configuration is incomplete.");

        using var client = new SmtpClient(host, configuration.GetValue("Email:Smtp:Port", 587))
        {
            EnableSsl = configuration.GetValue("Email:Smtp:EnableSsl", true),
            Credentials = string.IsNullOrWhiteSpace(username)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(username, password)
        };
        using var message = new MailMessage(from, recipient)
        {
            Subject = "Nuevo crédito registrado",
            Body = $"Cliente: {notification.ClientName}\nMonto: {notification.Amount:N0}\nComercial: {notification.CommercialName}\nFecha: {notification.RegisteredAtUtc:O}"
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
