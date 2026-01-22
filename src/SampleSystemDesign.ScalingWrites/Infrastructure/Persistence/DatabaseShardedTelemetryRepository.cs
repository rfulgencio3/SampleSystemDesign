namespace SampleSystemDesign.ScalingWrites.Infrastructure.Persistence;

using System.Globalization;
using Npgsql;
using SampleSystemDesign.ScalingWrites.Domain.Entities;
using SampleSystemDesign.ScalingWrites.Domain.Interfaces;

public sealed class DatabaseShardedTelemetryRepository : ITelemetryRepository
{
    private readonly string connectionString;
    private readonly int shardCount;

    public DatabaseShardedTelemetryRepository(string connectionString, int shardCount)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        if (shardCount <= 0) throw new ArgumentOutOfRangeException(nameof(shardCount));

        this.connectionString = connectionString;
        this.shardCount = shardCount;

        EnsureTables();
    }

    public async Task SaveBatchAsync(IReadOnlyCollection<TelemetryEvent> events, CancellationToken cancellationToken = default)
    {
        if (events is null) throw new ArgumentNullException(nameof(events));
        if (events.Count == 0) return;

        var groups = events.GroupBy(telemetryEvent => GetShardIndex(telemetryEvent.DeviceId));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var group in groups)
        {
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            foreach (var telemetryEvent in group)
            {
                await using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = $"INSERT INTO telemetry_events_{group.Key} (device_id, timestamp, metric_name, value) VALUES (@deviceId, @timestamp, @metricName, @value)";
                command.Parameters.AddWithValue("deviceId", telemetryEvent.DeviceId);
                command.Parameters.AddWithValue("timestamp", telemetryEvent.Timestamp);
                command.Parameters.AddWithValue("metricName", telemetryEvent.MetricName);
                command.Parameters.AddWithValue("value", telemetryEvent.Value);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }

    private int GetShardIndex(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId)) throw new ArgumentException("Device ID is required.", nameof(deviceId));

        return Math.Abs(deviceId.GetHashCode()) % shardCount;
    }

    private void EnsureTables()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        for (var shard = 0; shard < shardCount; shard++)
        {
            using var command = connection.CreateCommand();
            command.CommandText = string.Format(CultureInfo.InvariantCulture, """
                CREATE TABLE IF NOT EXISTS telemetry_events_{0} (
                    device_id text NOT NULL,
                    timestamp timestamptz NOT NULL,
                    metric_name text NOT NULL,
                    value double precision NOT NULL
                );
                """, shard);
            command.ExecuteNonQuery();
        }
    }
}
