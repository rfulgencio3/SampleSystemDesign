using System.Threading.Channels;
using SampleSystemDesign.LongRunning.Application.Interfaces;

namespace SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;

public sealed class InMemoryImageJobQueue : IImageJobQueue
{
    private readonly Channel<Guid> channel;

    public InMemoryImageJobQueue(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };

        channel = Channel.CreateBounded<Guid>(options);
    }

    public Task EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("Job ID is required.", nameof(jobId));

        return channel.Writer.WriteAsync(jobId, cancellationToken).AsTask();
    }

    public async Task<Guid> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return await channel.Reader.ReadAsync(cancellationToken);
    }
}
