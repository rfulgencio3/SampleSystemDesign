namespace SampleSystemDesign.Contention.Infrastructure.Persistence;

using Npgsql;
using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Domain.Interfaces;

public sealed class PostgresTicketInventoryRepository : ITicketInventoryRepository
{
    private readonly string connectionString;

    public PostgresTicketInventoryRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        this.connectionString = connectionString;
        EnsureTables();
    }

    public async Task<TicketInventory?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Event ID is required.", nameof(eventId));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, event_id, total_tickets, available_tickets, version
            FROM ticket_inventory
            WHERE event_id = @eventId
            LIMIT 1
            """;
        command.Parameters.AddWithValue("eventId", eventId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new TicketInventory(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetInt32(2),
            reader.GetInt32(3),
            reader.GetInt32(4));
    }

    public async Task SaveAsync(TicketInventory inventory, CancellationToken cancellationToken = default)
    {
        if (inventory is null) throw new ArgumentNullException(nameof(inventory));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO ticket_inventory (id, event_id, total_tickets, available_tickets, version)
            VALUES (@id, @eventId, @totalTickets, @availableTickets, @version)
            ON CONFLICT (event_id)
            DO UPDATE SET
                total_tickets = EXCLUDED.total_tickets,
                available_tickets = EXCLUDED.available_tickets,
                version = EXCLUDED.version
            """;
        command.Parameters.AddWithValue("id", inventory.Id);
        command.Parameters.AddWithValue("eventId", inventory.EventId);
        command.Parameters.AddWithValue("totalTickets", inventory.TotalTickets);
        command.Parameters.AddWithValue("availableTickets", inventory.AvailableTickets);
        command.Parameters.AddWithValue("version", inventory.Version);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<bool> TryUpdateAsync(TicketInventory updatedInventory, int expectedVersion, CancellationToken cancellationToken = default)
    {
        if (updatedInventory is null) throw new ArgumentNullException(nameof(updatedInventory));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE ticket_inventory
            SET available_tickets = @availableTickets,
                version = @newVersion
            WHERE event_id = @eventId AND version = @expectedVersion
            """;
        command.Parameters.AddWithValue("availableTickets", updatedInventory.AvailableTickets);
        command.Parameters.AddWithValue("newVersion", updatedInventory.Version);
        command.Parameters.AddWithValue("eventId", updatedInventory.EventId);
        command.Parameters.AddWithValue("expectedVersion", expectedVersion);

        var affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected > 0;
    }

    private void EnsureTables()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS ticket_inventory (
                id uuid PRIMARY KEY,
                event_id uuid NOT NULL UNIQUE,
                total_tickets integer NOT NULL,
                available_tickets integer NOT NULL,
                version integer NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
