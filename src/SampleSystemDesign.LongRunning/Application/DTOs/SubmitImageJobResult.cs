namespace SampleSystemDesign.LongRunning.Application.DTOs;

using SampleSystemDesign.LongRunning.Domain.Entities;

public sealed record SubmitImageJobResult(Guid JobId, ImageJobStatus Status);
