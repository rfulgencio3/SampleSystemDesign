namespace SampleSystemDesign.ScalingWrites.Domain.Entities;

public sealed class TelemetryEvent
{
    public TelemetryEvent(string deviceId, DateTimeOffset timestamp, string metricName, double value)
    {
        if (string.IsNullOrWhiteSpace(deviceId)) throw new ArgumentException("Device ID is required.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(metricName)) throw new ArgumentException("Metric name is required.", nameof(metricName));
        if (!double.IsFinite(value)) throw new ArgumentException("Metric value must be finite.", nameof(value));

        DeviceId = deviceId;
        Timestamp = timestamp;
        MetricName = metricName;
        Value = value;
    }

    public string DeviceId { get; }
    public DateTimeOffset Timestamp { get; }
    public string MetricName { get; }
    public double Value { get; }
}
