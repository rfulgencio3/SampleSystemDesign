namespace SampleSystemDesign.LongRunning.Application.Interfaces;

public interface IImageProcessor
{
    Task<string> ProcessAsync(string originalFileUrl, CancellationToken cancellationToken = default);
}
