using System.Collections.Concurrent;
using SampleSystemDesign.LargeFiles.Domain.Entities;
using SampleSystemDesign.LargeFiles.Domain.Interfaces;

namespace SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

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
        ArgumentNullException.ThrowIfNull(asset);

        assets[asset.Id] = asset;
        return Task.CompletedTask;
    }
}
