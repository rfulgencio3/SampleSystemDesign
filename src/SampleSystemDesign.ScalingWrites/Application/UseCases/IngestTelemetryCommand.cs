namespace SampleSystemDesign.ScalingWrites.Application.UseCases;

public sealed record IngestTelemetryCommand(string DeviceId, string MetricName, double Value, DateTimeOffset? Timestamp);
