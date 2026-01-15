namespace SampleSystemDesign.ScalingWrites.Presentation;

public sealed record TelemetryIngestRequest(string DeviceId, string MetricName, double Value, DateTimeOffset? Timestamp);
