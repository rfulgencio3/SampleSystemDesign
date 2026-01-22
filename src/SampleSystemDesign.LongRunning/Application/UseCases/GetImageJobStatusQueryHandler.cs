using SampleSystemDesign.LongRunning.Application.DTOs;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

namespace SampleSystemDesign.LongRunning.Application.UseCases;

public sealed class GetImageJobStatusQueryHandler(IImageJobRepository repository)
{
    private readonly IImageJobRepository repository = repository;

    public async Task<ImageJobStatusResult> HandleAsync(GetImageJobStatusQuery query, CancellationToken cancellationToken = default)
    {
        if (query.JobId == Guid.Empty) throw new ArgumentException("Job ID is required.", nameof(query));

        var job = await repository.GetByIdAsync(query.JobId, cancellationToken);

        return job is null
            ? ImageJobStatusResult.NotFound()
            : ImageJobStatusResult.FromJob(job);
    }
}
