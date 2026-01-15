namespace SampleSystemDesign.ScalingWrites.Domain.Interfaces;

using SampleSystemDesign.ScalingWrites.Domain.Entities;

public interface ITelemetryRepository
{
    Task SaveBatchAsync(IReadOnlyCollection<TelemetryEvent> events, CancellationToken cancellationToken = default);
}
