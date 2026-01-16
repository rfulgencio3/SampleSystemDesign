namespace SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

using System.Collections.Concurrent;
using SampleSystemDesign.LargeFiles.Domain.Entities;
using SampleSystemDesign.LargeFiles.Domain.Interfaces;

public sealed class InMemoryAssetRepository : IAssetRepository
{
    private readonly ConcurrentDictionary<Guid, Asset> assets = new();

    public Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        assets.TryGetValue(id, out var asset);
        return Task.FromResult(asset);
    }

    public Task SaveAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        if (asset is null) throw new ArgumentNullException(nameof(asset));

        assets[asset.Id] = asset;
        return Task.CompletedTask;
    }
}
