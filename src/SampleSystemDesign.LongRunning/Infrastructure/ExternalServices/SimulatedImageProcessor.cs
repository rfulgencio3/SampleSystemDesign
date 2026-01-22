using SampleSystemDesign.LongRunning.Application.Interfaces;

namespace SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;

public sealed class SimulatedImageProcessor : IImageProcessor
{
    private readonly TimeSpan processingTime;
    private readonly string resultBaseUrl;

    public SimulatedImageProcessor(TimeSpan processingTime, string resultBaseUrl)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(processingTime, TimeSpan.Zero);
        if (string.IsNullOrWhiteSpace(resultBaseUrl)) throw new ArgumentException("Result base URL is required.", nameof(resultBaseUrl));

        this.processingTime = processingTime;
        this.resultBaseUrl = resultBaseUrl.TrimEnd('/');
    }

    public async Task<string> ProcessAsync(string originalFileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(originalFileUrl)) throw new ArgumentException("Original file URL is required.", nameof(originalFileUrl));

        await Task.Delay(processingTime, cancellationToken);

        var token = Guid.NewGuid().ToString("N");
        return $"{resultBaseUrl}/images/{token}.jpg";
    }
}
