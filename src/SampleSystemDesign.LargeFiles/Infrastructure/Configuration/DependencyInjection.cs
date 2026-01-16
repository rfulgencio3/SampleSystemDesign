namespace SampleSystemDesign.LargeFiles.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.LargeFiles.Application.Interfaces;
using SampleSystemDesign.LargeFiles.Application.UseCases;
using SampleSystemDesign.LargeFiles.Domain.Interfaces;
using SampleSystemDesign.LargeFiles.Infrastructure.ExternalServices;
using SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddLargeFiles(this IServiceCollection services)
    {
        services.AddSingleton<IAssetRepository, InMemoryAssetRepository>();
        services.AddSingleton<IStorageService>(_ => new SimulatedStorageService("https://storage.example.com", TimeSpan.FromMinutes(15)));
        services.AddSingleton<GenerateUploadUrlCommandHandler>();
        services.AddSingleton<GenerateDownloadUrlQueryHandler>();

        return services;
    }
}
