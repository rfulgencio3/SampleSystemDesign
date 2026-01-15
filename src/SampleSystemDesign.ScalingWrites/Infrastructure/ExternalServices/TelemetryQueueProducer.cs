namespace SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;

using SampleSystemDesign.ScalingWrites.Application.Interfaces;
using SampleSystemDesign.ScalingWrites.Domain.Entities;

public sealed class TelemetryQueueProducer : ITelemetryQueueProducer
{
    private readonly TelemetryQueue queue;

    public TelemetryQueueProducer(TelemetryQueue queue)
    {
        this.queue = queue;
    }

    public Task EnqueueAsync(TelemetryEvent telemetryEvent, CancellationToken cancellationToken = default)
    {
        if (telemetryEvent is null) throw new ArgumentNullException(nameof(telemetryEvent));

        return queue.Writer.WriteAsync(telemetryEvent, cancellationToken).AsTask();
    }
}
