namespace SampleSystemDesign.LargeFiles.Infrastructure.ExternalServices;

using SampleSystemDesign.LargeFiles.Application.Interfaces;

public sealed class SimulatedStorageService : IStorageService
{
    private readonly string baseUrl;
    private readonly TimeSpan urlTtl;

    public SimulatedStorageService(string baseUrl, TimeSpan urlTtl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Base URL is required.", nameof(baseUrl));
        if (urlTtl <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(urlTtl));

        this.baseUrl = baseUrl.TrimEnd('/');
        this.urlTtl = urlTtl;
    }

    public Task<string> GenerateUploadUrlAsync(string storagePath, string contentType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path is required.", nameof(storagePath));
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("Content type is required.", nameof(contentType));

        var encodedPath = Uri.EscapeDataString(storagePath);
        var encodedContentType = Uri.EscapeDataString(contentType);
        var token = Guid.NewGuid().ToString("N");
        var expiresAt = DateTimeOffset.UtcNow.Add(urlTtl).ToUnixTimeSeconds();

        var url = $"{baseUrl}/upload/{encodedPath}?token={token}&expires={expiresAt}&contentType={encodedContentType}";
        return Task.FromResult(url);
    }

    public Task<string> GenerateDownloadUrlAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path is required.", nameof(storagePath));

        var encodedPath = Uri.EscapeDataString(storagePath);
        var token = Guid.NewGuid().ToString("N");
        var expiresAt = DateTimeOffset.UtcNow.Add(urlTtl).ToUnixTimeSeconds();

        var url = $"{baseUrl}/download/{encodedPath}?token={token}&expires={expiresAt}";
        return Task.FromResult(url);
    }
}
