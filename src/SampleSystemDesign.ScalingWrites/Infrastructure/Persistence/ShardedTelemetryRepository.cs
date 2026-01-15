namespace SampleSystemDesign.ScalingWrites.Infrastructure.Persistence;

using System.Collections.Concurrent;
using SampleSystemDesign.ScalingWrites.Domain.Entities;
using SampleSystemDesign.ScalingWrites.Domain.Interfaces;

public sealed class ShardedTelemetryRepository : ITelemetryRepository
{
    private readonly int shardCount;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<TelemetryEvent>>[] shards;

    public ShardedTelemetryRepository(int shardCount)
    {
        if (shardCount <= 0) throw new ArgumentOutOfRangeException(nameof(shardCount));

        this.shardCount = shardCount;
        shards = new ConcurrentDictionary<string, ConcurrentQueue<TelemetryEvent>>[shardCount];

        for (var index = 0; index < shardCount; index++)
        {
            shards[index] = new ConcurrentDictionary<string, ConcurrentQueue<TelemetryEvent>>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public Task SaveBatchAsync(IReadOnlyCollection<TelemetryEvent> events, CancellationToken cancellationToken = default)
    {
        if (events is null) throw new ArgumentNullException(nameof(events));

        foreach (var telemetryEvent in events)
        {
            var shard = shards[GetShardIndex(telemetryEvent.DeviceId)];
            var deviceQueue = shard.GetOrAdd(telemetryEvent.DeviceId, _ => new ConcurrentQueue<TelemetryEvent>());
            deviceQueue.Enqueue(telemetryEvent);
        }

        return Task.CompletedTask;
    }

    public int GetShardIndex(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId)) throw new ArgumentException("Device ID is required.", nameof(deviceId));

        return Math.Abs(deviceId.GetHashCode()) % shardCount;
    }

    public int GetDeviceEventCount(string deviceId)
    {
        var shard = shards[GetShardIndex(deviceId)];
        return shard.TryGetValue(deviceId, out var queue) ? queue.Count : 0;
    }
}
