using SampleSystemDesign.LargeFiles.Application.UseCases;
using SampleSystemDesign.LargeFiles.Infrastructure.ExternalServices;
using SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

namespace SampleSystemDesign.LargeFiles.Tests.Application.UseCases;

public class GenerateDownloadUrlQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsNotFoundWhenMissing()
    {
        var repository = new InMemoryAssetRepository();
        var storage = new SimulatedStorageService("https://storage.test", TimeSpan.FromMinutes(10));
        var handler = new GenerateDownloadUrlQueryHandler(repository, storage);

        var result = await handler.HandleAsync(new GenerateDownloadUrlQuery(Guid.NewGuid()));

        Assert.False(result.Found);
        Assert.Null(result.DownloadUrl);
    }

    [Fact]
    public async Task HandleAsync_ReturnsUrlForExistingAsset()
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
