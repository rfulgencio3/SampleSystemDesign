namespace SampleSystemDesign.ScalingReads.Infrastructure.Persistence;

using Npgsql;
using SampleSystemDesign.ScalingReads.Domain.Entities;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;

public sealed class PostgresShortUrlRepository : IShortUrlRepository
{
    private readonly string connectionString;

    public PostgresShortUrlRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        this.connectionString = connectionString;
        EnsureTable();
    }

    public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            throw new ArgumentException("Short code is required.", nameof(shortCode));
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, original_url, short_code, expiration_date
            FROM short_urls
            WHERE short_code = @shortCode
            LIMIT 1
            """;
        command.Parameters.AddWithValue("shortCode", shortCode);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var id = reader.GetGuid(0);
        var originalUrl = reader.GetString(1);
        var code = reader.GetString(2);
        var expiration = reader.IsDBNull(3) ? (DateTimeOffset?)null : reader.GetFieldValue<DateTimeOffset>(3);

        return new ShortUrl(id, originalUrl, code, expiration);
    }

    public async Task SaveAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default)
    {
        if (shortUrl is null) throw new ArgumentNullException(nameof(shortUrl));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO short_urls (id, original_url, short_code, expiration_date)
            VALUES (@id, @originalUrl, @shortCode, @expirationDate)
            ON CONFLICT (short_code)
            DO UPDATE SET
                original_url = EXCLUDED.original_url,
                expiration_date = EXCLUDED.expiration_date
            """;
        command.Parameters.AddWithValue("id", shortUrl.Id);
        command.Parameters.AddWithValue("originalUrl", shortUrl.OriginalUrl);
        command.Parameters.AddWithValue("shortCode", shortUrl.ShortCode);
        command.Parameters.AddWithValue("expirationDate", (object?)shortUrl.ExpirationDate ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private void EnsureTable()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS short_urls (
                id uuid PRIMARY KEY,
                original_url text NOT NULL,
                short_code text NOT NULL UNIQUE,
                expiration_date timestamptz NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
