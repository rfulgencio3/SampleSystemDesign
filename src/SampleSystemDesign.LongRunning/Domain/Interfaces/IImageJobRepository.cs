namespace SampleSystemDesign.LongRunning.Domain.Interfaces;

using SampleSystemDesign.LongRunning.Domain.Entities;

public interface IImageJobRepository
{
    Task<ImageJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveAsync(ImageJob job, CancellationToken cancellationToken = default);
    Task UpdateAsync(ImageJob job, CancellationToken cancellationToken = default);
}
