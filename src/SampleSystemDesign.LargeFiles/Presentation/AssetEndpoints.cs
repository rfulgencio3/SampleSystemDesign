namespace SampleSystemDesign.LargeFiles.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SampleSystemDesign.LargeFiles.Application.UseCases;

public static class AssetEndpoints
{
    public static IEndpointRouteBuilder MapAssetEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/assets/upload-url", async (GenerateUploadUrlRequest request, GenerateUploadUrlCommandHandler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(
                new GenerateUploadUrlCommand(request.FileName, request.ContentType, request.UploadedBy),
                cancellationToken);

            return Results.Ok(new GenerateUploadUrlResponse(result.AssetId, result.UploadUrl, result.StoragePath));
        });

        endpoints.MapGet("/api/assets/{assetId:guid}/download-url", async (Guid assetId, GenerateDownloadUrlQueryHandler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GenerateDownloadUrlQuery(assetId), cancellationToken);

            return result.Found
                ? Results.Ok(new GenerateDownloadUrlResponse(assetId, result.DownloadUrl!))
                : Results.NotFound();
        });

        return endpoints;
    }
}
