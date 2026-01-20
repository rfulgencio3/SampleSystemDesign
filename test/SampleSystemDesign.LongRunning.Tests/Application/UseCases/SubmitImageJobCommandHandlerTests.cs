using SampleSystemDesign.LongRunning.Application.Interfaces;
using SampleSystemDesign.LongRunning.Application.UseCases;
using SampleSystemDesign.LongRunning.Domain.Entities;
using SampleSystemDesign.LongRunning.Infrastructure.Persistence;

namespace SampleSystemDesign.LongRunning.Tests.Application.UseCases;

public class SubmitImageJobCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_EnqueuesJob()
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
}
