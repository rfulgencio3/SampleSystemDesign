namespace SampleSystemDesign.LongRunning.Application.DTOs;

using SampleSystemDesign.LongRunning.Domain.Entities;

public sealed record ImageJobStatusResult(bool Found, ImageJobStatus? Status, string? ResultFileUrl)
{
    public static ImageJobStatusResult NotFound() => new(false, null, null);

    public static ImageJobStatusResult FromJob(ImageJob job) =>
        new(true, job.Status, job.ResultFileUrl);
}
