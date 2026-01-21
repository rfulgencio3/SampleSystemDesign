namespace SampleSystemDesign.ScalingWrites.Infrastructure.Configuration;

public sealed record RabbitMqSettings(string HostName, int Port, string UserName, string Password, string Queue);
