using System.Collections.Concurrent;
using SampleSystemDesign.LongRunning.Domain.Entities;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

namespace SampleSystemDesign.LongRunning.Infrastructure.Persistence;

public sealed class InMemoryImageJobRepository : IImageJobRepository
{
    private readonly ConcurrentDictionary<Guid, ImageJob> jobs = new();

    public Task<ImageJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        jobs.TryGetValue(id, out var job);
        return Task.FromResult(job);
    }

    public Task SaveAsync(ImageJob job, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job);

        jobs[job.Id] = job;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ImageJob job, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job);

        jobs[job.Id] = job;
        return Task.CompletedTask;
    }
}
