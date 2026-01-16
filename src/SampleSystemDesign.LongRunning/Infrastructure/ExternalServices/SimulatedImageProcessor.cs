namespace SampleSystemDesign.LongRunning.Infrastructure.ExternalServices;

using SampleSystemDesign.LongRunning.Application.Interfaces;

public sealed class SimulatedImageProcessor : IImageProcessor
{
    private readonly TimeSpan processingTime;
    private readonly string resultBaseUrl;

    public SimulatedImageProcessor(TimeSpan processingTime, string resultBaseUrl)
    {
        if (processingTime <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(processingTime));
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
