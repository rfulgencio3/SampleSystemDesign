using SampleSystemDesign.ScalingReads.Application.Interfaces;
using SampleSystemDesign.ScalingReads.Domain.Entities;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;
using SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;

namespace SampleSystemDesign.ScalingReads.Tests.Infrastructure.ExternalServices;

public class RedisShortUrlCacheTests
{
    [Fact]
    public async Task GetByShortCodeAsync_ReturnsCachedValueWithoutDatabaseCall()
    {
        var clock = new FakeClock(DateTimeOffset.UtcNow);
        var cache = new InMemoryShortUrlCache(TimeSpan.FromMinutes(5));
        var database = new FakeShortUrlRepository();
        var cached = new ShortUrl(Guid.NewGuid(), "https://example.com", "abc", clock.UtcNow.AddMinutes(10));
        cache.Set(cached, clock.UtcNow);

        var repository = new RedisShortUrlCache(database, cache, clock);

        var result = await repository.GetByShortCodeAsync("abc");

        Assert.NotNull(result);
        Assert.Equal(cached.OriginalUrl, result!.OriginalUrl);
        Assert.Equal(0, database.GetCalls);
    }

    [Fact]
    public async Task GetByShortCodeAsync_PopulatesCacheOnMiss()
    {
        var clock = new FakeClock(DateTimeOffset.UtcNow);
        var cache = new InMemoryShortUrlCache(TimeSpan.FromMinutes(5));
        var database = new FakeShortUrlRepository();
        var shortUrl = new ShortUrl(Guid.NewGuid(), "https://example.com/one", "one", clock.UtcNow.AddHours(1));
        database.Save(shortUrl);

        var repository = new RedisShortUrlCache(database, cache, clock);

        var first = await repository.GetByShortCodeAsync("one");
        var second = await repository.GetByShortCodeAsync("one");

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Equal(1, database.GetCalls);
    }

    private sealed class FakeClock : IClock
    {
        public FakeClock(DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTimeOffset UtcNow { get; }
    }

    private sealed class FakeShortUrlRepository : IShortUrlRepository
    {
        private readonly Dictionary<string, ShortUrl> entries = new(StringComparer.OrdinalIgnoreCase);

        public int GetCalls { get; private set; }

        public Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            GetCalls++;
            entries.TryGetValue(shortCode, out var shortUrl);
            return Task.FromResult(shortUrl);
        }

        public Task SaveAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default)
        {
            entries[shortUrl.ShortCode] = shortUrl;
            return Task.CompletedTask;
        }

        public void Save(ShortUrl shortUrl) => entries[shortUrl.ShortCode] = shortUrl;
    }
}
