namespace SampleSystemDesign.LongRunning.Presentation;

public sealed record SubmitImageJobResponse(Guid JobId, string Status);
