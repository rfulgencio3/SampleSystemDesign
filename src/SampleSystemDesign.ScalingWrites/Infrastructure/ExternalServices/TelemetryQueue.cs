namespace SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;

using System.Threading.Channels;
using SampleSystemDesign.ScalingWrites.Domain.Entities;

public sealed class TelemetryQueue
{
    private readonly Channel<TelemetryEvent> channel;

    public TelemetryQueue(int capacity)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };

        channel = Channel.CreateBounded<TelemetryEvent>(options);
    }

    public ChannelWriter<TelemetryEvent> Writer => channel.Writer;
    public ChannelReader<TelemetryEvent> Reader => channel.Reader;
}
