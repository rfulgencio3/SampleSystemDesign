namespace SampleSystemDesign.ScalingWrites.Application.UseCases;

using SampleSystemDesign.ScalingWrites.Application.Interfaces;
using SampleSystemDesign.ScalingWrites.Domain.Entities;

public sealed class IngestTelemetryCommandHandler
{
    private readonly ITelemetryQueueProducer producer;

    public IngestTelemetryCommandHandler(ITelemetryQueueProducer producer)
    {
        this.producer = producer;
    }

    public async Task HandleAsync(IngestTelemetryCommand command, CancellationToken cancellationToken = default)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        if (string.IsNullOrWhiteSpace(command.DeviceId))
        {
            throw new ArgumentException("Device ID is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.MetricName))
        {
            throw new ArgumentException("Metric name is required.", nameof(command));
        }

        if (!double.IsFinite(command.Value))
        {
            throw new ArgumentException("Metric value must be finite.", nameof(command));
        }

        var timestamp = command.Timestamp ?? DateTimeOffset.UtcNow;
        var telemetryEvent = new TelemetryEvent(command.DeviceId, timestamp, command.MetricName, command.Value);

        await producer.EnqueueAsync(telemetryEvent, cancellationToken);
    }
}
