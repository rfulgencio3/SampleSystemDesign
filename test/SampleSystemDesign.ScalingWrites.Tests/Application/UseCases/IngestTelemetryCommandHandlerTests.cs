using SampleSystemDesign.ScalingWrites.Application.Interfaces;
using SampleSystemDesign.ScalingWrites.Application.UseCases;
using SampleSystemDesign.ScalingWrites.Domain.Entities;

namespace SampleSystemDesign.ScalingWrites.Tests.Application.UseCases;

public class IngestTelemetryCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_EnqueuesTelemetryEvent()
    {
        var producer = new FakeTelemetryQueueProducer();
        var handler = new IngestTelemetryCommandHandler(producer);
        var timestamp = DateTimeOffset.UtcNow;

        await handler.HandleAsync(new IngestTelemetryCommand("device-1", "temperature", 21.5, timestamp));

        Assert.Single(producer.Events);
        Assert.Equal("device-1", producer.Events[0].DeviceId);
        Assert.Equal("temperature", producer.Events[0].MetricName);
        Assert.Equal(21.5, producer.Events[0].Value);
        Assert.Equal(timestamp, producer.Events[0].Timestamp);
    }

    [Fact]
    public async Task HandleAsync_ThrowsOnInvalidDeviceId()
    {
        var producer = new FakeTelemetryQueueProducer();
        var handler = new IngestTelemetryCommandHandler(producer);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(new IngestTelemetryCommand(" ", "temperature", 21.5, DateTimeOffset.UtcNow)));
    }

    private sealed class FakeTelemetryQueueProducer : ITelemetryQueueProducer
    {
        public List<TelemetryEvent> Events { get; } = new();

        public Task EnqueueAsync(TelemetryEvent telemetryEvent, CancellationToken cancellationToken = default)
        {
            Events.Add(telemetryEvent);
            return Task.CompletedTask;
        }
    }
}
