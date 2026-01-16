namespace SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;

using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

public sealed class ImageProcessingWorker : BackgroundService
{
    private readonly IImageJobQueue queue;
    private readonly IImageJobRepository repository;
    private readonly IImageProcessor processor;

    public ImageProcessingWorker(IImageJobQueue queue, IImageJobRepository repository, IImageProcessor processor)
    {
        this.queue = queue;
        this.repository = repository;
        this.processor = processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Guid jobId;
            try
            {
                jobId = await queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            var job = await repository.GetByIdAsync(jobId, stoppingToken);
            if (job is null)
            {
                continue;
            }

            var processing = job.MarkProcessing();
            await repository.UpdateAsync(processing, stoppingToken);

            try
            {
                var resultUrl = await processor.ProcessAsync(processing.OriginalFileUrl, stoppingToken);
                var completed = processing.MarkCompleted(resultUrl);
                await repository.UpdateAsync(completed, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                var failed = processing.MarkFailed();
                await repository.UpdateAsync(failed, stoppingToken);
            }
        }
    }
}
