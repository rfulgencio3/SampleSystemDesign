namespace SampleSystemDesign.RealTime.Infrastructure.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.RealTime.Application.Interfaces;
using SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

public static class DependencyInjection
{
    public static IServiceCollection AddRealTime(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var intervalSeconds = ParseInt(configuration["MarketData:IntervalSeconds"], 2);

        services.AddSignalR();
        services.AddSingleton<IMarketDataPublisher, MarketDataPublisher>();
        services.AddHostedService(sp => new MarketDataGeneratorService(
            sp.GetRequiredService<IMarketDataPublisher>(),
            TimeSpan.FromSeconds(intervalSeconds)));

        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
