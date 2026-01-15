using SampleSystemDesign.ScalingWrites.Application.Interfaces;
using SampleSystemDesign.ScalingWrites.Application.UseCases;
using SampleSystemDesign.ScalingWrites.Domain.Entities;
using SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;
using SampleSystemDesign.ScalingWrites.Infrastructure.Persistence;

namespace SampleSystemDesign.ScalingWrites.Tests;

public class ScalingWritesTests
{
    [Fact]
    public async Task Handler_EnqueuesTelemetryEvent()
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
    public async Task Handler_ThrowsOnInvalidDeviceId()
    {
        var producer = new FakeTelemetryQueueProducer();
        var handler = new IngestTelemetryCommandHandler(producer);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(new IngestTelemetryCommand(" ", "temperature", 21.5, DateTimeOffset.UtcNow)));
    }

    [Fact]
    public async Task QueueConsumer_PersistsEventsFromQueue()
    {
        var repository = new ShardedTelemetryRepository(2);
        var queue = new TelemetryQueue(10);
        var consumer = new TelemetryQueueConsumer(queue, repository, batchSize: 2);
        var now = DateTimeOffset.UtcNow;

        await consumer.StartAsync(CancellationToken.None);

        await queue.Writer.WriteAsync(new TelemetryEvent("device-1", now, "temp", 10));
        await queue.Writer.WriteAsync(new TelemetryEvent("device-1", now, "temp", 11));

        await Task.Delay(50);
        await consumer.StopAsync(CancellationToken.None);

        Assert.Equal(2, repository.GetDeviceEventCount("device-1"));
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
