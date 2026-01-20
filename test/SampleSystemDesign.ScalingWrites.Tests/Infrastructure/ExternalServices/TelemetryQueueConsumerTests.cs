using SampleSystemDesign.ScalingWrites.Domain.Entities;
using SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;
using SampleSystemDesign.ScalingWrites.Infrastructure.Persistence;

namespace SampleSystemDesign.ScalingWrites.Tests.Infrastructure.ExternalServices;

public class TelemetryQueueConsumerTests
{
    [Fact]
    public async Task ExecuteAsync_PersistsEventsFromQueue()
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
}
