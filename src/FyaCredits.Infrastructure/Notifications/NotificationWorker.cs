using FyaCredits.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FyaCredits.Infrastructure.Notifications;

public sealed class NotificationWorker(
    NotificationQueue queue,
    IEmailSender emailSender,
    ILogger<NotificationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var notification in queue.ReadAllAsync(stoppingToken))
        {
            for (var attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    await emailSender.SendCreditRegisteredAsync(notification, stoppingToken);
                    break;
                }
                catch (Exception exception) when (attempt < 3)
                {
                    logger.LogWarning(
                        exception,
                        "Credit notification attempt {Attempt} failed for credit {CreditId}.",
                        attempt,
                        notification.CreditId);
                    await Task.Delay(TimeSpan.FromSeconds(attempt), stoppingToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "Credit notification permanently failed for credit {CreditId}.",
                        notification.CreditId);
                }
            }
        }
    }
}
