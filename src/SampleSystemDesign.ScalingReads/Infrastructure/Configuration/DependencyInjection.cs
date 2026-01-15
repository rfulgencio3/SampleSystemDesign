namespace SampleSystemDesign.ScalingReads.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.ScalingReads.Application.Interfaces;
using SampleSystemDesign.ScalingReads.Application.UseCases;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;
using SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;
using SampleSystemDesign.ScalingReads.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddScalingReads(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IShortUrlCache>(_ => new InMemoryShortUrlCache(TimeSpan.FromMinutes(10)));
        services.AddSingleton<InMemoryShortUrlRepository>();
        services.AddSingleton<IShortUrlRepository>(sp =>
            new RedisShortUrlCache(
                sp.GetRequiredService<InMemoryShortUrlRepository>(),
                sp.GetRequiredService<IShortUrlCache>(),
                sp.GetRequiredService<IClock>()));
        services.AddSingleton<GetOriginalUrlQueryHandler>();

        return services;
    }
}
