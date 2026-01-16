namespace SampleSystemDesign.LargeFiles.Application.UseCases;

public sealed record GenerateUploadUrlCommand(string FileName, string ContentType, string UploadedBy);
