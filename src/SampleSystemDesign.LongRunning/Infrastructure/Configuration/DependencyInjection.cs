namespace SampleSystemDesign.LongRunning.Infrastructure.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Application.UseCases;
using SampleSystemDesign.LongRunning.Domain.Interfaces;
using SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;
using SampleSystemDesign.LongRunning.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddLongRunning(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required.");

        var host = configuration["RabbitMq:HostName"] ?? "localhost";
        var port = ParseInt(configuration["RabbitMq:Port"], 5672);
        var user = configuration["RabbitMq:UserName"] ?? "guest";
        var password = configuration["RabbitMq:Password"] ?? "guest";
        var queue = configuration["RabbitMq:Queue"] ?? "image.jobs";
        var delaySeconds = ParseInt(configuration["Processing:DelaySeconds"], 2);

        services.AddSingleton<IImageJobRepository>(_ => new PostgresImageJobRepository(connectionString));
        services.AddSingleton<IImageJobQueue>(_ => new RabbitMqImageJobQueue(host, port, user, password, queue));
        services.AddSingleton<IImageProcessor>(_ => new SimulatedImageProcessor(TimeSpan.FromSeconds(delaySeconds), "https://images.example.com"));
        services.AddSingleton<SubmitImageJobCommandHandler>();
        services.AddSingleton<GetImageJobStatusQueryHandler>();
        services.AddHostedService<ImageProcessingWorker>();

        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
