using SampleSystemDesign.LargeFiles.Application.UseCases;
using SampleSystemDesign.LargeFiles.Infrastructure.ExternalServices;
using SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

namespace SampleSystemDesign.LargeFiles.Tests.Application.UseCases;

public class GenerateUploadUrlCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_SavesAssetAndReturnsUrl()
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
}
