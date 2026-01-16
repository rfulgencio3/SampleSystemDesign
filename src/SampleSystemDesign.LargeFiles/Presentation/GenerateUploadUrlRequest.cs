namespace SampleSystemDesign.LargeFiles.Presentation;

public sealed record GenerateUploadUrlRequest(string FileName, string ContentType, string UploadedBy);
