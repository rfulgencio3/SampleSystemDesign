namespace SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;

using System.Text;
using RabbitMQ.Client;
using SampleSystemDesign.LongRunning.Application.Interfaces;

public sealed class RabbitMqImageJobQueue : IImageJobQueue, IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;

    public RabbitMqImageJobQueue(string host, int port, string userName, string password, string queueName)
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

    public Task EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("Job ID is required.", nameof(jobId));

        var body = Encoding.UTF8.GetBytes(jobId.ToString("D"));
        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: props, body: body);
        return Task.CompletedTask;
    }

    public async Task<Guid> DequeueAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = channel.BasicGet(queueName, autoAck: false);
            if (result is not null)
            {
                var bodyText = Encoding.UTF8.GetString(result.Body.Span);
                channel.BasicAck(result.DeliveryTag, multiple: false);

                if (Guid.TryParse(bodyText, out var jobId))
                {
                    return jobId;
                }
            }

            await Task.Delay(200, cancellationToken);
        }

        throw new OperationCanceledException(cancellationToken);
    }

    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }
}
