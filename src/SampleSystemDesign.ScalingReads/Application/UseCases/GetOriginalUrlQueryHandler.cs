namespace SampleSystemDesign.ScalingReads.Application.UseCases;

using SampleSystemDesign.ScalingReads.Application.DTOs;
using SampleSystemDesign.ScalingReads.Application.Interfaces;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;

public sealed class GetOriginalUrlQueryHandler
{
    private readonly IShortUrlRepository repository;
    private readonly IClock clock;

    public GetOriginalUrlQueryHandler(IShortUrlRepository repository, IClock clock)
    {
        this.repository = repository;
        this.clock = clock;
    }

    public async Task<GetOriginalUrlResult> HandleAsync(GetOriginalUrlQuery query, CancellationToken cancellationToken = default)
    {
        if (query is null) throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrWhiteSpace(query.ShortCode))
        {
            throw new ArgumentException("Short code is required.", nameof(query));
        }

        var shortUrl = await repository.GetByShortCodeAsync(query.ShortCode, cancellationToken);

        if (shortUrl is null || shortUrl.IsExpired(clock.UtcNow))
        {
            return GetOriginalUrlResult.NotFound();
        }

        return GetOriginalUrlResult.FromUrl(shortUrl.OriginalUrl);
    }
}
