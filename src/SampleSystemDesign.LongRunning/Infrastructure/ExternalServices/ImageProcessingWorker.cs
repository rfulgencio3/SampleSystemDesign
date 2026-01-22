using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

namespace SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;

public sealed class ImageProcessingWorker(IImageJobQueue queue, IImageJobRepository repository, IImageProcessor processor) : BackgroundService
{
    private readonly IImageJobQueue queue = queue;
    private readonly IImageJobRepository repository = repository;
    private readonly IImageProcessor processor = processor;

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
