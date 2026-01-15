namespace SampleSystemDesign.ScalingReads.Infrastructure.Persistence;

using System.Collections.Concurrent;
using SampleSystemDesign.ScalingReads.Domain.Entities;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;

public sealed class InMemoryShortUrlRepository : IShortUrlRepository
{
    private readonly ConcurrentDictionary<string, ShortUrl> entries = new(StringComparer.OrdinalIgnoreCase);

    public Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            throw new ArgumentException("Short code is required.", nameof(shortCode));
        }

        entries.TryGetValue(shortCode, out var shortUrl);
        return Task.FromResult(shortUrl);
    }

    public Task SaveAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default)
    {
        if (shortUrl is null) throw new ArgumentNullException(nameof(shortUrl));

        entries[shortUrl.ShortCode] = shortUrl;
        return Task.CompletedTask;
    }

    public void Seed(IEnumerable<ShortUrl> shortUrls)
    {
        foreach (var shortUrl in shortUrls)
        {
            entries[shortUrl.ShortCode] = shortUrl;
        }
    }
}
