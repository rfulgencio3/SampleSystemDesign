namespace SampleSystemDesign.LongRunning.Infrastructure.Persistence;

using System.Collections.Concurrent;
using SampleSystemDesign.LongRunning.Domain.Entities;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

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
        if (job is null) throw new ArgumentNullException(nameof(job));

        jobs[job.Id] = job;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ImageJob job, CancellationToken cancellationToken = default)
    {
        if (job is null) throw new ArgumentNullException(nameof(job));

        jobs[job.Id] = job;
        return Task.CompletedTask;
    }
}
