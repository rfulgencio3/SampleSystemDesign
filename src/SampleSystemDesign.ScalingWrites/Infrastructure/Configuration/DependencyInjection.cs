namespace SampleSystemDesign.ScalingWrites.Infrastructure.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.ScalingWrites.Application.Interfaces;
using SampleSystemDesign.ScalingWrites.Application.UseCases;
using SampleSystemDesign.ScalingWrites.Domain.Interfaces;
using SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;
using SampleSystemDesign.ScalingWrites.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddScalingWrites(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required.");

        var shardCount = ParseInt(configuration["Telemetry:ShardCount"], 4);
        var batchSize = ParseInt(configuration["Telemetry:BatchSize"], 100);

        var rabbitSettings = GetRabbitMqSettings(configuration);

        services.AddSingleton(rabbitSettings);
        services.AddSingleton<ITelemetryQueueProducer, RabbitMqTelemetryQueueProducer>();
        services.AddSingleton<ITelemetryRepository>(_ => new PostgresShardedTelemetryRepository(connectionString, shardCount));
        services.AddSingleton<IngestTelemetryCommandHandler>();
        services.AddHostedService(sp => new RabbitMqTelemetryQueueConsumer(rabbitSettings, sp.GetRequiredService<ITelemetryRepository>(), batchSize));

        return services;
    }

    private static RabbitMqSettings GetRabbitMqSettings(IConfiguration configuration)
    {
        var host = configuration["RabbitMq:HostName"] ?? "localhost";
        var port = ParseInt(configuration["RabbitMq:Port"], 5672);
        var user = configuration["RabbitMq:UserName"] ?? "guest";
        var password = configuration["RabbitMq:Password"] ?? "guest";
        var queue = configuration["RabbitMq:Queue"] ?? "telemetry.events";

        return new RabbitMqSettings(host, port, user, password, queue);
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
