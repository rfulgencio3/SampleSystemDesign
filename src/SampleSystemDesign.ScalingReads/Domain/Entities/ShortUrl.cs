namespace SampleSystemDesign.ScalingReads.Domain.Entities;

public sealed class ShortUrl
{
    public ShortUrl(Guid id, string originalUrl, string shortCode, DateTimeOffset? expirationDate)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(originalUrl)) throw new ArgumentException("Original URL is required.", nameof(originalUrl));
        if (string.IsNullOrWhiteSpace(shortCode)) throw new ArgumentException("Short code is required.", nameof(shortCode));

        Id = id;
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        ExpirationDate = expirationDate;
    }

    public Guid Id { get; }
    public string OriginalUrl { get; }
    public string ShortCode { get; }
    public DateTimeOffset? ExpirationDate { get; }

    public bool IsExpired(DateTimeOffset now) =>
        ExpirationDate.HasValue && ExpirationDate.Value <= now;
}
