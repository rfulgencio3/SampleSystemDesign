namespace SampleSystemDesign.ScalingWrites.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.ScalingWrites.Application.Interfaces;
using SampleSystemDesign.ScalingWrites.Application.UseCases;
using SampleSystemDesign.ScalingWrites.Domain.Interfaces;
using SampleSystemDesign.ScalingWrites.Infrastructure.ExternalServices;
using SampleSystemDesign.ScalingWrites.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddScalingWrites(this IServiceCollection services)
    {
        services.AddSingleton(new TelemetryQueue(1000));
        services.AddSingleton<ITelemetryQueueProducer, TelemetryQueueProducer>();
        services.AddSingleton<ITelemetryRepository>(_ => new ShardedTelemetryRepository(4));
        services.AddSingleton<IngestTelemetryCommandHandler>();
        services.AddHostedService<TelemetryQueueConsumer>();

        return services;
    }
}
