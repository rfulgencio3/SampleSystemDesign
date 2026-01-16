namespace SampleSystemDesign.LargeFiles.Presentation;

public sealed record GenerateUploadUrlResponse(Guid AssetId, string UploadUrl, string StoragePath);
