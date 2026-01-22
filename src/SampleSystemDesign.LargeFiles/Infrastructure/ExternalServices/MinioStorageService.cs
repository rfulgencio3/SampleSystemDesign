using Minio;
using Minio.DataModel.Args;
using SampleSystemDesign.LargeFiles.Application.Interfaces;

namespace SampleSystemDesign.LargeFiles.Infrastructure.ExternalServices;

public sealed class MinioStorageService : IStorageService
{
    private readonly IMinioClient client;
    private readonly string bucket;
    private readonly TimeSpan urlTtl;
    private readonly SemaphoreSlim bucketLock = new(1, 1);
    private bool bucketReady;

    public MinioStorageService(IMinioClient client, string bucket, TimeSpan urlTtl)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(bucket)) throw new ArgumentException("Bucket is required.", nameof(bucket));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(urlTtl, TimeSpan.Zero);

        this.client = client;
        this.bucket = bucket;
        this.urlTtl = urlTtl;
    }

    public async Task<string> GenerateUploadUrlAsync(string storagePath, string contentType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path is required.", nameof(storagePath));
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("Content type is required.", nameof(contentType));

        await EnsureBucketAsync(cancellationToken);

        var args = new PresignedPutObjectArgs()
            .WithBucket(bucket)
            .WithObject(storagePath)
            .WithExpiry((int)urlTtl.TotalSeconds);

        return await client.PresignedPutObjectAsync(args);
    }

    public async Task<string> GenerateDownloadUrlAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path is required.", nameof(storagePath));

        await EnsureBucketAsync(cancellationToken);

        var args = new PresignedGetObjectArgs()
            .WithBucket(bucket)
            .WithObject(storagePath)
            .WithExpiry((int)urlTtl.TotalSeconds);

        return await client.PresignedGetObjectAsync(args);
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        if (bucketReady) return;

        await bucketLock.WaitAsync(cancellationToken);
        try
        {
            if (bucketReady) return;

            var existsArgs = new BucketExistsArgs().WithBucket(bucket);
            var exists = await client.BucketExistsAsync(existsArgs, cancellationToken);
            if (!exists)
            {
                var makeArgs = new MakeBucketArgs().WithBucket(bucket);
                await client.MakeBucketAsync(makeArgs, cancellationToken);
            }

            bucketReady = true;
        }
        finally
        {
            bucketLock.Release();
        }
    }
}
