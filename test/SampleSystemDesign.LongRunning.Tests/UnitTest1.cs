using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Application.UseCases;
using SampleSystemDesign.LongRunning.Domain.Entities;
using SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;
using SampleSystemDesign.LongRunning.Infrastructure.Persistence;

namespace SampleSystemDesign.LongRunning.Tests;

public class LongRunningTests
{
    [Fact]
    public async Task SubmitHandler_EnqueuesJob()
    {
        var repository = new InMemoryImageJobRepository();
        var queue = new FakeImageJobQueue();
        var handler = new SubmitImageJobCommandHandler(repository, queue);

        var result = await handler.HandleAsync(new SubmitImageJobCommand("https://files.example.com/image.jpg"));
        var job = await repository.GetByIdAsync(result.JobId);

        Assert.NotNull(job);
        Assert.Single(queue.EnqueuedJobIds);
        Assert.Equal(result.JobId, queue.EnqueuedJobIds[0]);
        Assert.Equal(ImageJobStatus.Pending, job!.Status);
    }

    [Fact]
    public async Task StatusHandler_ReturnsNotFoundWhenMissing()
    {
        var repository = new InMemoryImageJobRepository();
        var handler = new GetImageJobStatusQueryHandler(repository);

        var result = await handler.HandleAsync(new GetImageJobStatusQuery(Guid.NewGuid()));

        Assert.False(result.Found);
        Assert.Null(result.Status);
    }

    [Fact]
    public async Task Worker_ProcessesJobAndUpdatesStatus()
    {
        var repository = new InMemoryImageJobRepository();
        var queue = new InMemoryImageJobQueue(5);
        var processor = new TestImageProcessor("https://processed.example.com");
        var worker = new ImageProcessingWorker(queue, repository, processor);

        var job = new ImageJob(Guid.NewGuid(), "https://files.example.com/raw.jpg", ImageJobStatus.Pending, null);
        await repository.SaveAsync(job);
        await queue.EnqueueAsync(job.Id);

        await worker.StartAsync(CancellationToken.None);

        var completed = await WaitForStatusAsync(repository, job.Id, ImageJobStatus.Completed, TimeSpan.FromSeconds(2));

        await worker.StopAsync(CancellationToken.None);

        Assert.NotNull(completed.ResultFileUrl);
        Assert.Equal(1, processor.CallCount);
    }

    private static async Task<ImageJob> WaitForStatusAsync(
        InMemoryImageJobRepository repository,
        Guid jobId,
        ImageJobStatus expectedStatus,
        TimeSpan timeout)
    {
        var expiresAt = DateTimeOffset.UtcNow.Add(timeout);

        while (DateTimeOffset.UtcNow < expiresAt)
        {
            var job = await repository.GetByIdAsync(jobId);
            if (job is not null && job.Status == expectedStatus)
            {
                return job;
            }

            await Task.Delay(20);
        }

        throw new TimeoutException("Job did not reach the expected status in time.");
    }

    private sealed class FakeImageJobQueue : IImageJobQueue
    {
        public List<Guid> EnqueuedJobIds { get; } = new();

        public Task EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            EnqueuedJobIds.Add(jobId);
            return Task.CompletedTask;
        }

        public Task<Guid> DequeueAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Dequeue is not used in this test.");
        }
    }

    private sealed class TestImageProcessor : IImageProcessor
    {
        private readonly string baseUrl;

        public TestImageProcessor(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
        }

        public int CallCount { get; private set; }

        public Task<string> ProcessAsync(string originalFileUrl, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult($"{baseUrl}/images/{Guid.NewGuid():N}.jpg");
        }
    }
}
