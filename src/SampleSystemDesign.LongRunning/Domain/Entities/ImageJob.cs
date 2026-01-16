namespace SampleSystemDesign.LongRunning.Domain.Entities;

public sealed class ImageJob
{
    public ImageJob(Guid id, string originalFileUrl, ImageJobStatus status, string? resultFileUrl)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(originalFileUrl)) throw new ArgumentException("Original file URL is required.", nameof(originalFileUrl));

        Id = id;
        OriginalFileUrl = originalFileUrl;
        Status = status;
        ResultFileUrl = resultFileUrl;
    }

    public Guid Id { get; }
    public string OriginalFileUrl { get; }
    public ImageJobStatus Status { get; }
    public string? ResultFileUrl { get; }

    public ImageJob MarkProcessing() => new(Id, OriginalFileUrl, ImageJobStatus.Processing, null);

    public ImageJob MarkCompleted(string resultFileUrl)
    {
        if (string.IsNullOrWhiteSpace(resultFileUrl))
        {
            throw new ArgumentException("Result file URL is required.", nameof(resultFileUrl));
        }

        return new ImageJob(Id, OriginalFileUrl, ImageJobStatus.Completed, resultFileUrl);
    }

    public ImageJob MarkFailed() => new(Id, OriginalFileUrl, ImageJobStatus.Failed, null);
}
