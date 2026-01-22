using SampleSystemDesign.LargeFiles.Application.DTOs;
using SampleSystemDesign.LargeFiles.Application.Interfaces;
using SampleSystemDesign.LargeFiles.Domain.Entities;
using SampleSystemDesign.LargeFiles.Domain.Interfaces;

namespace SampleSystemDesign.LargeFiles.Application.UseCases;

public sealed class GenerateUploadUrlCommandHandler(IAssetRepository assetRepository, IStorageService storageService)
{
    private readonly IAssetRepository assetRepository = assetRepository;
    private readonly IStorageService storageService = storageService;

    public async Task<UploadUrlResult> HandleAsync(GenerateUploadUrlCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.FileName)) throw new ArgumentException("File name is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.ContentType)) throw new ArgumentException("Content type is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.UploadedBy)) throw new ArgumentException("Uploaded by is required.", nameof(command));

        var safeFileName = Path.GetFileName(command.FileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            throw new ArgumentException("File name is invalid.", nameof(command));
        }

        var assetId = Guid.NewGuid();
        var storagePath = $"{command.UploadedBy}/{assetId:N}-{safeFileName}";

        var uploadUrl = await storageService.GenerateUploadUrlAsync(storagePath, command.ContentType, cancellationToken);

        var asset = new Asset(assetId, safeFileName, command.ContentType, storagePath, command.UploadedBy);
        await assetRepository.SaveAsync(asset, cancellationToken);

        return new UploadUrlResult(asset.Id, uploadUrl, asset.StoragePath);
    }
}
