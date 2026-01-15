namespace SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;

using Microsoft.Extensions.Hosting;
using SampleSystemDesign.ScalingWrites.Domain.Entities;
using SampleSystemDesign.ScalingWrites.Domain.Interfaces;

public sealed class TelemetryQueueConsumer : BackgroundService
{
    private readonly TelemetryQueue queue;
    private readonly ITelemetryRepository repository;
    private readonly int batchSize;

    public TelemetryQueueConsumer(TelemetryQueue queue, ITelemetryRepository repository, int batchSize = 100)
    {
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        this.queue = queue;
        this.repository = repository;
        this.batchSize = batchSize;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<TelemetryEvent>(batchSize);

        while (await queue.Reader.WaitToReadAsync(stoppingToken))
        {
            while (queue.Reader.TryRead(out var telemetryEvent))
            {
                batch.Add(telemetryEvent);

                if (batch.Count >= batchSize)
                {
                    await FlushAsync(batch, stoppingToken);
                }
            }

            if (batch.Count > 0)
            {
                await FlushAsync(batch, stoppingToken);
            }
        }
    }

    private Task FlushAsync(List<TelemetryEvent> batch, CancellationToken stoppingToken)
    {
        var snapshot = batch.ToArray();
        batch.Clear();
        return repository.SaveBatchAsync(snapshot, stoppingToken);
    }
}
