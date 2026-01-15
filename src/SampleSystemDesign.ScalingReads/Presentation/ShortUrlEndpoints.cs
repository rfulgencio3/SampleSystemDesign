namespace SampleSystemDesign.ScalingReads.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SampleSystemDesign.ScalingReads.Application.UseCases;

public static class ShortUrlEndpoints
{
    public static IEndpointRouteBuilder MapShortUrlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/r/{shortCode}", async (string shortCode, GetOriginalUrlQueryHandler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetOriginalUrlQuery(shortCode), cancellationToken);
            return result.Found ? Results.Redirect(result.OriginalUrl!) : Results.NotFound();
        });

        return endpoints;
    }
}
