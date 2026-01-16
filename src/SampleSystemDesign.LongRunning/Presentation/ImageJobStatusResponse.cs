namespace SampleSystemDesign.LongRunning.Presentation;

public sealed record ImageJobStatusResponse(Guid JobId, string Status, string? ResultFileUrl);
