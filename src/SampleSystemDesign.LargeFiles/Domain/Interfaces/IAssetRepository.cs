namespace SampleSystemDesign.LargeFiles.Domain.Interfaces;

using SampleSystemDesign.LargeFiles.Domain.Entities;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveAsync(Asset asset, CancellationToken cancellationToken = default);
}
