namespace SampleSystemDesign.LargeFiles.Infrastructure.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using SampleSystemDesign.LargeFiles.Application.Interfaces;
using SampleSystemDesign.LargeFiles.Application.UseCases;
using SampleSystemDesign.LargeFiles.Domain.Interfaces;
using SampleSystemDesign.LargeFiles.Infrastructure.ExternalServices;
using SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddLargeFiles(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required.");

        var endpoint = configuration["Minio:Endpoint"] ?? "localhost:9000";
        var accessKey = configuration["Minio:AccessKey"] ?? "minio";
        var secretKey = configuration["Minio:SecretKey"] ?? "minio123";
        var bucket = configuration["Minio:Bucket"] ?? "large-files";
        var secure = ParseBool(configuration["Minio:Secure"]);
        var ttlMinutes = ParseInt(configuration["Storage:UrlTtlMinutes"], 15);

        var client = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey);

        if (secure)
        {
            client = client.WithSSL();
        }

        services.AddSingleton<IMinioClient>(client.Build());
        services.AddSingleton<IAssetRepository>(_ => new PostgresAssetRepository(connectionString));
        services.AddSingleton<IStorageService>(sp => new MinioStorageService(sp.GetRequiredService<IMinioClient>(), bucket, TimeSpan.FromMinutes(ttlMinutes)));
        services.AddSingleton<GenerateUploadUrlCommandHandler>();
        services.AddSingleton<GenerateDownloadUrlQueryHandler>();

        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }

    private static bool ParseBool(string? value)
    {
        return bool.TryParse(value, out var parsed) && parsed;
    }
}
