using FyaCredits.Application;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace FyaCredits.Infrastructure.Notifications;

public sealed class SmtpEmailSender(IConfiguration configuration, IEmailTemplateRenderer renderer) : IEmailSender
{
    public async Task SendCreditRegisteredAsync(
        CreditRegisteredNotification notification,
        CancellationToken cancellationToken)
    {
        if (!configuration.GetValue("Email:Enabled", false))
            return;

        var host = configuration["Email:Smtp:Host"];
        var from = configuration["Email:From"];
        var recipient = configuration["Email:To"];
        var username = configuration["Email:Smtp:Username"];
        var password = configuration["Email:Smtp:Password"];

        if (string.IsNullOrWhiteSpace(host)
            || string.IsNullOrWhiteSpace(from)
            || string.IsNullOrWhiteSpace(recipient))
            throw new InvalidOperationException("Email SMTP configuration is incomplete.");

        var port = configuration.GetValue("Email:Smtp:Port", 587);
        var secureOptions = configuration.GetValue("Email:Smtp:EnableSsl", true)
            ? port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable
            : SecureSocketOptions.None;

        var htmlBody = await renderer.RenderAsync(notification, cancellationToken);

        var builder = new BodyBuilder
        {
            TextBody = $"Cliente: {notification.ClientName}\n" +
                       $"Cédula o ID: {notification.ClientId}\n" +
                       $"Monto: {notification.Amount:N0}\n" +
                       $"Tasa de interés (NM): {notification.InterestRate}%\n" +
                       $"Plazo: {notification.TermMonths} meses\n" +
                       $"Comercial: {notification.CommercialName}\n" +
                       $"Fecha: {notification.RegisteredAtUtc:O}"
        };
        if (!string.IsNullOrWhiteSpace(htmlBody))
            builder.HtmlBody = htmlBody;

        var message = new MimeMessage
        {
            From = { MailboxAddress.Parse(from) },
            To = { MailboxAddress.Parse(recipient) },
            Subject = "Nuevo crédito registrado",
            Body = builder.ToMessageBody()
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, secureOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(username))
        {
            client.AuthenticationMechanisms.Clear();
            client.AuthenticationMechanisms.Add("LOGIN");
            client.AuthenticationMechanisms.Add("PLAIN");
            await client.AuthenticateAsync(username, password, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
