namespace SampleSystemDesign.LargeFiles.Infrastructure.Persistence;

using Npgsql;
using SampleSystemDesign.LargeFiles.Domain.Entities;
using SampleSystemDesign.LargeFiles.Domain.Interfaces;

public sealed class DatabaseAssetRepository : IAssetRepository
{
    private readonly string connectionString;

    public DatabaseAssetRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        this.connectionString = connectionString;
        EnsureTable();
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, file_name, content_type, storage_path, uploaded_by
            FROM assets
            WHERE id = @id
            LIMIT 1
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new Asset(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4));
    }

    public async Task SaveAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(asset);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO assets (id, file_name, content_type, storage_path, uploaded_by)
            VALUES (@id, @fileName, @contentType, @storagePath, @uploadedBy)
            ON CONFLICT (id)
            DO UPDATE SET
                file_name = EXCLUDED.file_name,
                content_type = EXCLUDED.content_type,
                storage_path = EXCLUDED.storage_path,
                uploaded_by = EXCLUDED.uploaded_by
            """;
        command.Parameters.AddWithValue("id", asset.Id);
        command.Parameters.AddWithValue("fileName", asset.FileName);
        command.Parameters.AddWithValue("contentType", asset.ContentType);
        command.Parameters.AddWithValue("storagePath", asset.StoragePath);
        command.Parameters.AddWithValue("uploadedBy", asset.UploadedBy);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private void EnsureTable()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS assets (
                id uuid PRIMARY KEY,
                file_name text NOT NULL,
                content_type text NOT NULL,
                storage_path text NOT NULL,
                uploaded_by text NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
