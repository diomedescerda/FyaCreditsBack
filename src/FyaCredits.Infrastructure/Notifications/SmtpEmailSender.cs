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
        var html = await renderer.RenderCreditAsync(notification, cancellationToken);
        var text = $"Cliente: {notification.ClientName}\n" +
                   $"Cédula o ID: {notification.ClientId}\n" +
                   $"Monto: {notification.Amount:N0}\n" +
                   $"Tasa de interés (NM): {notification.InterestRate}%\n" +
                   $"Plazo: {notification.TermMonths} meses\n" +
                   $"Comercial: {notification.CommercialName}\n" +
                   $"Fecha: {notification.RegisteredAtUtc:O}";

        await SendAsync(
            configuration["Email:To"] ?? string.Empty,
            "Nuevo crédito registrado",
            text,
            html,
            cancellationToken);
    }

    public async Task SendPasswordResetAsync(
        string email,
        string resetUrl,
        CancellationToken cancellationToken)
    {
        var html = await renderer.RenderPasswordResetAsync(email, resetUrl, cancellationToken);
        var text = $"Para restablecer tu contraseña, abre el siguiente enlace:\n{resetUrl}\n\n" +
                   "Si no solicitaste este cambio, ignora este correo.";

        await SendAsync(
            email,
            "Restablece tu contraseña",
            text,
            html,
            cancellationToken);
    }

    private async Task SendAsync(
        string recipient,
        string subject,
        string text,
        string? html,
        CancellationToken cancellationToken)
    {
        if (!configuration.GetValue("Email:Enabled", false))
            return;

        var host = configuration["Email:Smtp:Host"];
        var from = configuration["Email:From"];
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

        var builder = new BodyBuilder { TextBody = text };
        if (!string.IsNullOrWhiteSpace(html))
            builder.HtmlBody = html;

        var message = new MimeMessage
        {
            From = { MailboxAddress.Parse(from) },
            To = { MailboxAddress.Parse(recipient) },
            Subject = subject,
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
