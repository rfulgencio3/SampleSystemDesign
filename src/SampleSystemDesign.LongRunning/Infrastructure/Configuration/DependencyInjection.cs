namespace SampleSystemDesign.LongRunning.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Application.UseCases;
using SampleSystemDesign.LongRunning.Domain.Interfaces;
using SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;
using SampleSystemDesign.LongRunning.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddLongRunning(this IServiceCollection services)
    {
        services.AddSingleton<IImageJobRepository, InMemoryImageJobRepository>();
        services.AddSingleton<IImageJobQueue>(_ => new InMemoryImageJobQueue(500));
        services.AddSingleton<IImageProcessor>(_ => new SimulatedImageProcessor(TimeSpan.FromSeconds(2), "https://images.example.com"));
        services.AddSingleton<SubmitImageJobCommandHandler>();
        services.AddSingleton<GetImageJobStatusQueryHandler>();
        services.AddHostedService<ImageProcessingWorker>();

        return services;
    }
}
