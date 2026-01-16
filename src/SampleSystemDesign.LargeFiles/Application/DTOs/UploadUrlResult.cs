namespace SampleSystemDesign.LargeFiles.Application.DTOs;

public sealed record UploadUrlResult(Guid AssetId, string UploadUrl, string StoragePath);
