namespace SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;

using SampleSystemDesign.ScalingReads.Domain.Entities;

public interface IShortUrlCache
{
    bool TryGet(string shortCode, DateTimeOffset now, out ShortUrl? shortUrl);
    void Set(ShortUrl shortUrl, DateTimeOffset now);
}
