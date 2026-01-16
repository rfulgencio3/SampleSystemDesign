namespace SampleSystemDesign.LongRunning.Application.Interfaces;

public interface IImageJobQueue
{
    Task EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Guid> DequeueAsync(CancellationToken cancellationToken = default);
}
