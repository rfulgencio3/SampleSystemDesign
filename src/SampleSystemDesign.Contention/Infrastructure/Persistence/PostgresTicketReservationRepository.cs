namespace SampleSystemDesign.Contention.Infrastructure.Persistence;

using Npgsql;
using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Domain.Interfaces;

public sealed class PostgresTicketReservationRepository : ITicketReservationRepository
{
    private readonly string connectionString;

    public PostgresTicketReservationRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        this.connectionString = connectionString;
        EnsureTables();
    }

    public async Task SaveAsync(TicketReservation reservation, CancellationToken cancellationToken = default)
    {
        if (reservation is null) throw new ArgumentNullException(nameof(reservation));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO ticket_reservations (ticket_id, user_id, expiration_time)
            VALUES (@ticketId, @userId, @expirationTime)
            """;
        command.Parameters.AddWithValue("ticketId", reservation.TicketId);
        command.Parameters.AddWithValue("userId", reservation.UserId);
        command.Parameters.AddWithValue("expirationTime", reservation.ExpirationTime);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private void EnsureTables()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS ticket_reservations (
                id bigserial PRIMARY KEY,
                ticket_id uuid NOT NULL,
                user_id text NOT NULL,
                expiration_time timestamptz NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
