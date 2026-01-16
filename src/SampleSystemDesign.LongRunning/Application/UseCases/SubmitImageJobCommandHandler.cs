namespace SampleSystemDesign.LongRunning.Application.UseCases;

using SampleSystemDesign.LongRunning.Application.DTOs;
using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Domain.Entities;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

public sealed class SubmitImageJobCommandHandler
{
    private readonly IImageJobRepository repository;
    private readonly IImageJobQueue queue;

    public SubmitImageJobCommandHandler(IImageJobRepository repository, IImageJobQueue queue)
    {
        this.repository = repository;
        this.queue = queue;
    }

    public async Task<SubmitImageJobResult> HandleAsync(SubmitImageJobCommand command, CancellationToken cancellationToken = default)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
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
