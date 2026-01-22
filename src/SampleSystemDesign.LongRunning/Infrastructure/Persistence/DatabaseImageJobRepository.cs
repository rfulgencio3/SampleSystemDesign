using Npgsql;
using SampleSystemDesign.LongRunning.Domain.Entities;
using SampleSystemDesign.LongRunning.Domain.Interfaces;

namespace SampleSystemDesign.LongRunning.Infrastructure.Persistence;

public sealed class DatabaseImageJobRepository : IImageJobRepository
{
    private readonly string connectionString;

    public DatabaseImageJobRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        this.connectionString = connectionString;
        EnsureTable();
    }

    public async Task<ImageJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, original_file_url, status, result_file_url
            FROM image_jobs
            WHERE id = @id
            LIMIT 1
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var statusText = reader.GetString(2);
        var status = Enum.Parse<ImageJobStatus>(statusText, ignoreCase: true);
        var resultUrl = reader.IsDBNull(3) ? null : reader.GetString(3);

        return new ImageJob(
            reader.GetGuid(0),
            reader.GetString(1),
            status,
            resultUrl);
    }

    public async Task SaveAsync(ImageJob job, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO image_jobs (id, original_file_url, status, result_file_url)
            VALUES (@id, @originalFileUrl, @status, @resultFileUrl)
            ON CONFLICT (id)
            DO UPDATE SET
                original_file_url = EXCLUDED.original_file_url,
                status = EXCLUDED.status,
                result_file_url = EXCLUDED.result_file_url
            """;
        command.Parameters.AddWithValue("id", job.Id);
        command.Parameters.AddWithValue("originalFileUrl", job.OriginalFileUrl);
        command.Parameters.AddWithValue("status", job.Status.ToString());
        command.Parameters.AddWithValue("resultFileUrl", (object?)job.ResultFileUrl ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public Task UpdateAsync(ImageJob job, CancellationToken cancellationToken = default)
    {
        return SaveAsync(job, cancellationToken);
    }

    private void EnsureTable()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS image_jobs (
                id uuid PRIMARY KEY,
                original_file_url text NOT NULL,
                status text NOT NULL,
                result_file_url text NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
