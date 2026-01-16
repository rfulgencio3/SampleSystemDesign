namespace SampleSystemDesign.LargeFiles.Application.DTOs;

public sealed record DownloadUrlResult(bool Found, string? DownloadUrl)
{
    public static DownloadUrlResult NotFound() => new(false, null);
    public static DownloadUrlResult FromUrl(string downloadUrl) => new(true, downloadUrl);
}
