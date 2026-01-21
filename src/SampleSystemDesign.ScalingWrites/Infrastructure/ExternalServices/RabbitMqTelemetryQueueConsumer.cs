namespace SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;

using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using SampleSystemDesign.ScalingWrites.Domain.Entities;
using SampleSystemDesign.ScalingWrites.Domain.Interfaces;
using SampleSystemDesign.ScalingWrites.Infrastructure.Configuration;

public sealed class RabbitMqTelemetryQueueConsumer : BackgroundService
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;
    private readonly ITelemetryRepository repository;
    private readonly int batchSize;
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public RabbitMqTelemetryQueueConsumer(RabbitMqSettings settings, ITelemetryRepository repository, int batchSize)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        this.repository = repository;
        this.batchSize = batchSize;

        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        queueName = settings.Queue;

        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<TelemetryEvent>(batchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = channel.BasicGet(queueName, autoAck: false);
            if (result is null)
            {
                if (batch.Count > 0)
                {
                    await FlushAsync(batch, stoppingToken);
                }

                await Task.Delay(200, stoppingToken);
                continue;
            }

            var telemetryEvent = JsonSerializer.Deserialize<TelemetryEvent>(result.Body.Span, jsonOptions);
            channel.BasicAck(result.DeliveryTag, multiple: false);

            if (telemetryEvent is not null)
            {
                batch.Add(telemetryEvent);
            }

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

    private Task FlushAsync(List<TelemetryEvent> batch, CancellationToken stoppingToken)
    {
        var snapshot = batch.ToArray();
        batch.Clear();
        return repository.SaveBatchAsync(snapshot, stoppingToken);
    }

    public override void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
        base.Dispose();
    }
}
