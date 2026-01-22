using SampleSystemDesign.LongRunning.Application.DTOs;
using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Domain.Entities;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

namespace SampleSystemDesign.LongRunning.Application.UseCases;

public sealed class SubmitImageJobCommandHandler(IImageJobRepository repository, IImageJobQueue queue)
{
    private readonly IImageJobRepository repository = repository;
    private readonly IImageJobQueue queue = queue;

    public async Task<SubmitImageJobResult> HandleAsync(SubmitImageJobCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.OriginalFileUrl))
        {
            throw new ArgumentException("Original file URL is required.", nameof(command));
        }

        var job = new ImageJob(Guid.NewGuid(), command.OriginalFileUrl, ImageJobStatus.Pending, null);

        await repository.SaveAsync(job, cancellationToken);
        await queue.EnqueueAsync(job.Id, cancellationToken);

        return new SubmitImageJobResult(job.Id, job.Status);
    }
}
