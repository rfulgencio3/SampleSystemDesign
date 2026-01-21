namespace SampleSystemDesign.ScalingReads.Infrastructure.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.ScalingReads.Application.Interfaces;
using SampleSystemDesign.ScalingReads.Application.UseCases;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;
using SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;
using SampleSystemDesign.ScalingReads.Infrastructure.Persistence;
using StackExchange.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddScalingReads(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required.");

        var redisConnection = configuration["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Redis connection string is required.");

        var instanceName = configuration["Redis:InstanceName"] ?? "scaling-reads";
        var ttlMinutes = ParseInt(configuration["Caching:DefaultTtlMinutes"], 10);

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
        services.AddSingleton<IShortUrlCache>(sp =>
            new RedisShortUrlCacheClient(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                instanceName,
                TimeSpan.FromMinutes(ttlMinutes)));
        services.AddSingleton<IShortUrlRepository>(sp =>
            new RedisShortUrlCache(
                new PostgresShortUrlRepository(connectionString),
                sp.GetRequiredService<IShortUrlCache>(),
                sp.GetRequiredService<IClock>()));
        services.AddSingleton<GetOriginalUrlQueryHandler>();

        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
