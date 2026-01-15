namespace SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;

using SampleSystemDesign.ScalingReads.Application.Interfaces;
using SampleSystemDesign.ScalingReads.Domain.Entities;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;

public sealed class RedisShortUrlCache : IShortUrlRepository
{
    private readonly IShortUrlRepository database;
    private readonly IShortUrlCache cache;
    private readonly IClock clock;

    public RedisShortUrlCache(IShortUrlRepository database, IShortUrlCache cache, IClock clock)
    {
        this.database = database;
        this.cache = cache;
        this.clock = clock;
    }

    public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            throw new ArgumentException("Short code is required.", nameof(shortCode));
        }

        if (cache.TryGet(shortCode, clock.UtcNow, out var cached))
        {
            return cached;
        }

        var fromDatabase = await database.GetByShortCodeAsync(shortCode, cancellationToken);

        if (fromDatabase is null)
        {
            return null;
        }

        cache.Set(fromDatabase, clock.UtcNow);
        return fromDatabase;
    }

    public async Task SaveAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default)
    {
        if (shortUrl is null) throw new ArgumentNullException(nameof(shortUrl));

        await database.SaveAsync(shortUrl, cancellationToken);
        cache.Set(shortUrl, clock.UtcNow);
    }
}
