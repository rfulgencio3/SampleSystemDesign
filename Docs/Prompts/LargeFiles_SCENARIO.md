# Scenario: AssetStorageService (Large Files)

## Business Goal
Manage the secure and efficient upload and download of large media files (e.g., event videos, high-res photos) without routing the file data through the application server.

## System Design Pattern: Large Files
The solution must implement the **Large Files** pattern using:
1.  **Presigned URLs:** The application server will generate temporary, time-limited URLs for direct client-to-storage interaction (simulated S3/Blob Storage).
2.  **Metadata Management:** The application server will only store the file metadata (name, size, storage path).

## Implementation Tasks (AI Agent Focus)
1.  **Domain:** Define the `Asset` Entity (Id, FileName, ContentType, StoragePath, UploadedBy).
2.  **Application:** Create the `GenerateUploadUrlCommandHandler` and `GenerateDownloadUrlQueryHandler`. These handlers will interact with the simulated storage service.
3.  **Infrastructure:** Implement an `IStorageService` interface with methods for `GenerateUploadUrlAsync` and `GenerateDownloadUrlAsync`. The implementation will simulate the logic of AWS S3 or Azure Blob Storage SDKs.
4.  **Presentation:** Create API endpoints to request the upload and download URLs.
