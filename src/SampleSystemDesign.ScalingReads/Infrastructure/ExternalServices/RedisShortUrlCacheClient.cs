namespace SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;

using System.Text.Json;
using SampleSystemDesign.ScalingReads.Domain.Entities;
using StackExchange.Redis;

public sealed class RedisShortUrlCacheClient : IShortUrlCache
{
    private readonly IDatabase database;
    private readonly string prefix;
    private readonly TimeSpan defaultTtl;
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public RedisShortUrlCacheClient(IConnectionMultiplexer connection, string instanceName, TimeSpan defaultTtl)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (string.IsNullOrWhiteSpace(instanceName)) throw new ArgumentException("Instance name is required.", nameof(instanceName));
        if (defaultTtl <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(defaultTtl));

        database = connection.GetDatabase();
        prefix = instanceName.Trim() + ":";
        this.defaultTtl = defaultTtl;
    }

    public bool TryGet(string shortCode, DateTimeOffset now, out ShortUrl? shortUrl)
    {
        shortUrl = null;

        if (string.IsNullOrWhiteSpace(shortCode))
        {
            return false;
        }

        var value = database.StringGet(prefix + shortCode);
        if (!value.HasValue)
        {
            return false;
        }

        var cached = JsonSerializer.Deserialize<CachedShortUrl>(value!, jsonOptions);
        if (cached is null)
        {
            database.KeyDelete(prefix + shortCode);
            return false;
        }

        shortUrl = new ShortUrl(cached.Id, cached.OriginalUrl, cached.ShortCode, cached.ExpirationDate);
        if (shortUrl.IsExpired(now))
        {
            database.KeyDelete(prefix + shortCode);
            shortUrl = null;
            return false;
        }

        return true;
    }

    public void Set(ShortUrl shortUrl, DateTimeOffset now)
    {
        if (shortUrl is null) throw new ArgumentNullException(nameof(shortUrl));

        var expiresAt = shortUrl.ExpirationDate ?? now.Add(defaultTtl);
        var ttl = expiresAt - now;
        if (ttl <= TimeSpan.Zero)
        {
            ttl = TimeSpan.FromSeconds(1);
        }

        var payload = new CachedShortUrl(shortUrl.Id, shortUrl.OriginalUrl, shortUrl.ShortCode, shortUrl.ExpirationDate);
        var json = JsonSerializer.Serialize(payload, jsonOptions);
        database.StringSet(prefix + shortUrl.ShortCode, json, ttl);
    }

    private sealed record CachedShortUrl(Guid Id, string OriginalUrl, string ShortCode, DateTimeOffset? ExpirationDate);
}
