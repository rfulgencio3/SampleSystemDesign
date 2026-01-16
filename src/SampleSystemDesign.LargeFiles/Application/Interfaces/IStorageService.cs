namespace SampleSystemDesign.LargeFiles.Application.Interfaces;

public interface IStorageService
{
    Task<string> GenerateUploadUrlAsync(string storagePath, string contentType, CancellationToken cancellationToken = default);
    Task<string> GenerateDownloadUrlAsync(string storagePath, CancellationToken cancellationToken = default);
}
