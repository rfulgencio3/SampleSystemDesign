namespace SampleSystemDesign.ScalingReads.Domain.Interfaces;

using SampleSystemDesign.ScalingReads.Domain.Entities;

public interface IShortUrlRepository
{
    Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);
    Task SaveAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default);
}
