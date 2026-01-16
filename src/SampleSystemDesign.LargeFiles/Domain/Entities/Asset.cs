namespace SampleSystemDesign.LargeFiles.Domain.Entities;

public sealed class Asset
{
    public Asset(Guid id, string fileName, string contentType, string storagePath, string uploadedBy)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("Content type is required.", nameof(contentType));
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path is required.", nameof(storagePath));
        if (string.IsNullOrWhiteSpace(uploadedBy)) throw new ArgumentException("Uploaded by is required.", nameof(uploadedBy));

        Id = id;
        FileName = fileName;
        ContentType = contentType;
        StoragePath = storagePath;
        UploadedBy = uploadedBy;
    }

    public Guid Id { get; }
    public string FileName { get; }
    public string ContentType { get; }
    public string StoragePath { get; }
    public string UploadedBy { get; }
}
