using SampleSystemDesign.LargeFiles.Application.UseCases;
using SampleSystemDesign.LargeFiles.Domain.Entities;
using SampleSystemDesign.LargeFiles.Infrastructure.ExternalServices;
using SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

namespace SampleSystemDesign.LargeFiles.Tests;

public class LargeFilesTests
{
    [Fact]
    public async Task UploadHandler_SavesAssetAndReturnsUrl()
    {
        var repository = new InMemoryAssetRepository();
        var storage = new SimulatedStorageService("https://storage.test", TimeSpan.FromMinutes(10));
        var handler = new GenerateUploadUrlCommandHandler(repository, storage);

        var result = await handler.HandleAsync(new GenerateUploadUrlCommand("photo.jpg", "image/jpeg", "user-1"));
        var asset = await repository.GetByIdAsync(result.AssetId);

        Assert.NotNull(asset);
        Assert.Equal("photo.jpg", asset!.FileName);
        Assert.Equal("image/jpeg", asset.ContentType);
        Assert.Equal("user-1", asset.UploadedBy);
        Assert.Equal(asset.StoragePath, result.StoragePath);
        Assert.Contains("storage.test", result.UploadUrl);
    }

    [Fact]
    public async Task DownloadHandler_ReturnsNotFoundWhenMissing()
    {
        var repository = new InMemoryAssetRepository();
        var storage = new SimulatedStorageService("https://storage.test", TimeSpan.FromMinutes(10));
        var handler = new GenerateDownloadUrlQueryHandler(repository, storage);

        var result = await handler.HandleAsync(new GenerateDownloadUrlQuery(Guid.NewGuid()));

        Assert.False(result.Found);
        Assert.Null(result.DownloadUrl);
    }

    [Fact]
    public async Task DownloadHandler_ReturnsUrlForExistingAsset()
    {
        var repository = new InMemoryAssetRepository();
        var storage = new SimulatedStorageService("https://storage.test", TimeSpan.FromMinutes(10));
        var uploadHandler = new GenerateUploadUrlCommandHandler(repository, storage);
        var downloadHandler = new GenerateDownloadUrlQueryHandler(repository, storage);

        var uploadResult = await uploadHandler.HandleAsync(new GenerateUploadUrlCommand("clip.mp4", "video/mp4", "user-2"));
        var downloadResult = await downloadHandler.HandleAsync(new GenerateDownloadUrlQuery(uploadResult.AssetId));

        Assert.True(downloadResult.Found);
        Assert.NotNull(downloadResult.DownloadUrl);
        Assert.Contains("download", downloadResult.DownloadUrl!);
    }
}
