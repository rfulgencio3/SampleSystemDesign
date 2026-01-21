namespace SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;

using System.Text.Json;
using RabbitMQ.Client;
using SampleSystemDesign.MultiStep.Application.DTOs;
using SampleSystemDesign.MultiStep.Application.Interfaces;

public sealed class RabbitMqCheckoutMessageBus : ICheckoutMessageBus, IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public RabbitMqCheckoutMessageBus(string host, int port, string userName, string password, string queueName)
    {
        if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("Host is required.", nameof(host));
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("User name is required.", nameof(userName));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.", nameof(password));
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException("Queue name is required.", nameof(queueName));

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = userName,
            Password = password
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        this.queueName = queueName;

        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
    }

    public Task PublishAsync(CheckoutMessage message, CancellationToken cancellationToken = default)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));

        var body = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);
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
