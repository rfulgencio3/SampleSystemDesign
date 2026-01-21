namespace SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;

using System.Text.Json;
using RabbitMQ.Client;
using SampleSystemDesign.ScalingWrites.Application.Interfaces;
using SampleSystemDesign.ScalingWrites.Domain.Entities;
using SampleSystemDesign.ScalingWrites.Infrastructure.Configuration;

public sealed class RabbitMqTelemetryQueueProducer : ITelemetryQueueProducer, IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public RabbitMqTelemetryQueueProducer(RabbitMqSettings settings)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));

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

    public Task EnqueueAsync(TelemetryEvent telemetryEvent, CancellationToken cancellationToken = default)
    {
        if (telemetryEvent is null) throw new ArgumentNullException(nameof(telemetryEvent));

        var body = JsonSerializer.SerializeToUtf8Bytes(telemetryEvent, jsonOptions);
        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: props, body: body);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }
}
