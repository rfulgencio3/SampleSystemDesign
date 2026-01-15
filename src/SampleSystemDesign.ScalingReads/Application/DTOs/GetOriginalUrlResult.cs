namespace SampleSystemDesign.ScalingReads.Application.DTOs;

public sealed record GetOriginalUrlResult(bool Found, string? OriginalUrl)
{
    public static GetOriginalUrlResult NotFound() => new(false, null);
    public static GetOriginalUrlResult FromUrl(string originalUrl) => new(true, originalUrl);
}
