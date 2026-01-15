namespace SampleSystemDesign.ScalingWrites.Application.Interfaces;

using SampleSystemDesign.ScalingWrites.Domain.Entities;

public interface ITelemetryQueueProducer
{
    Task EnqueueAsync(TelemetryEvent telemetryEvent, CancellationToken cancellationToken = default);
}
