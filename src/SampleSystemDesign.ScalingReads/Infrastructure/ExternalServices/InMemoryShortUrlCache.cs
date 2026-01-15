namespace SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;

using System.Collections.Concurrent;
using SampleSystemDesign.ScalingReads.Domain.Entities;

public sealed class InMemoryShortUrlCache : IShortUrlCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> entries = new(StringComparer.OrdinalIgnoreCase);
    private readonly TimeSpan defaultTtl;

    public InMemoryShortUrlCache(TimeSpan defaultTtl)
    {
        if (defaultTtl <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(defaultTtl));
        this.defaultTtl = defaultTtl;
    }

    public bool TryGet(string shortCode, DateTimeOffset now, out ShortUrl? shortUrl)
    {
        shortUrl = null;

        if (!entries.TryGetValue(shortCode, out var entry))
        {
            return false;
        }

        if (entry.ExpiresAt <= now)
        {
            entries.TryRemove(shortCode, out _);
            return false;
        }

        shortUrl = entry.ShortUrl;
        return true;
    }

    public void Set(ShortUrl shortUrl, DateTimeOffset now)
    {
        var expiresAt = shortUrl.ExpirationDate ?? now.Add(defaultTtl);
        entries[shortUrl.ShortCode] = new CacheEntry(shortUrl, expiresAt);
    }

    private sealed record CacheEntry(ShortUrl ShortUrl, DateTimeOffset ExpiresAt);
}
