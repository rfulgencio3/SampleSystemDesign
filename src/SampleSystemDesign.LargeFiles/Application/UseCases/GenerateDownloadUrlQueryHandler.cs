using SampleSystemDesign.LargeFiles.Application.DTOs;
using SampleSystemDesign.LargeFiles.Application.Interfaces;
using SampleSystemDesign.LargeFiles.Domain.Interfaces;

namespace SampleSystemDesign.LargeFiles.Application.UseCases;

public sealed class GenerateDownloadUrlQueryHandler(IAssetRepository assetRepository, IStorageService storageService)
{
    private readonly IAssetRepository assetRepository = assetRepository;
    private readonly IStorageService storageService = storageService;

    public async Task<DownloadUrlResult> HandleAsync(GenerateDownloadUrlQuery query, CancellationToken cancellationToken = default)
    {
        if (query.AssetId == Guid.Empty) throw new ArgumentException("Asset ID is required.", nameof(query));

        var asset = await assetRepository.GetByIdAsync(query.AssetId, cancellationToken);

        if (asset is null)
        {
            return DownloadUrlResult.NotFound();
        }

        var downloadUrl = await storageService.GenerateDownloadUrlAsync(asset.StoragePath, cancellationToken);
        return DownloadUrlResult.FromUrl(downloadUrl);
    }
}
