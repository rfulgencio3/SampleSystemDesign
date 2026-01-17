namespace SampleSystemDesign.RealTime.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.RealTime.Application.Interfaces;
using SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

public static class DependencyInjection
{
    public static IServiceCollection AddRealTime(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IMarketDataPublisher, MarketDataPublisher>();
        services.AddHostedService(_ => new MarketDataGeneratorService(
            _.GetRequiredService<IMarketDataPublisher>(),
            TimeSpan.FromSeconds(2)));

        return services;
    }
}
