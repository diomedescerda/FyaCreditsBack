using System.Threading.Channels;
using FyaCredits.Application;

namespace FyaCredits.Infrastructure.Notifications;

public sealed class NotificationQueue : INotificationQueue
{
    private readonly Channel<CreditRegisteredNotification> channel =
        Channel.CreateBounded<CreditRegisteredNotification>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(
        CreditRegisteredNotification notification,
        CancellationToken cancellationToken) => channel.Writer.WriteAsync(notification, cancellationToken);

    public IAsyncEnumerable<CreditRegisteredNotification> ReadAllAsync(CancellationToken cancellationToken) =>
        channel.Reader.ReadAllAsync(cancellationToken);
}
