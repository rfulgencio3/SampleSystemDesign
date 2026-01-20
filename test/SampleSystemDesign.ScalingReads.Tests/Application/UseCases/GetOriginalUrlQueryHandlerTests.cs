using SampleSystemDesign.ScalingReads.Application.Interfaces;
using SampleSystemDesign.ScalingReads.Application.UseCases;
using SampleSystemDesign.ScalingReads.Domain.Entities;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;

namespace SampleSystemDesign.ScalingReads.Tests.Application.UseCases;

public class GetOriginalUrlQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsNotFoundWhenExpired()
    {
        var now = DateTimeOffset.UtcNow;
        var clock = new FakeClock(now);
        var repository = new FakeShortUrlRepository();
        var expired = new ShortUrl(Guid.NewGuid(), "https://example.com/old", "old", now.AddMinutes(-1));
        repository.Save(expired);

        var handler = new GetOriginalUrlQueryHandler(repository, clock);

        var result = await handler.HandleAsync(new GetOriginalUrlQuery("old"));

        Assert.False(result.Found);
        Assert.Null(result.OriginalUrl);
    }

    [Fact]
    public async Task HandleAsync_ReturnsOriginalUrlWhenActive()
    {
        var now = DateTimeOffset.UtcNow;
        var clock = new FakeClock(now);
        var repository = new FakeShortUrlRepository();
        var active = new ShortUrl(Guid.NewGuid(), "https://example.com/active", "active", now.AddMinutes(5));
        repository.Save(active);

        var handler = new GetOriginalUrlQueryHandler(repository, clock);

        var result = await handler.HandleAsync(new GetOriginalUrlQuery("active"));

        Assert.True(result.Found);
        Assert.Equal(active.OriginalUrl, result.OriginalUrl);
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

        public Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
        {
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
