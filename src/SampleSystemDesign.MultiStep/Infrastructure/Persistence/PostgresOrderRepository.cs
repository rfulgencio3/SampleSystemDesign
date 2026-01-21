namespace SampleSystemDesign.MultiStep.Infrastructure.Persistence;

using System.Text.Json;
using Npgsql;
using SampleSystemDesign.MultiStep.Domain.Entities;
using SampleSystemDesign.MultiStep.Domain.Interfaces;

public sealed class PostgresOrderRepository : IOrderRepository
{
    private readonly string connectionString;
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public PostgresOrderRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        this.connectionString = connectionString;
        EnsureTable();
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, status, total, items
            FROM orders
            WHERE id = @id
            LIMIT 1
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var statusText = reader.GetString(1);
        var status = Enum.Parse<OrderStatus>(statusText, ignoreCase: true);
        var total = reader.GetDecimal(2);
        var itemsJson = reader.GetString(3);
        var itemsPayload = JsonSerializer.Deserialize<List<OrderItemPayload>>(itemsJson, jsonOptions) ?? new List<OrderItemPayload>();
        var items = itemsPayload.Select(item => new OrderItem(item.Sku, item.Quantity, item.UnitPrice)).ToArray();

        return new Order(reader.GetGuid(0), status, items, total);
    }

    public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO orders (id, status, total, items)
            VALUES (@id, @status, @total, @items)
            ON CONFLICT (id)
            DO UPDATE SET
                status = EXCLUDED.status,
                total = EXCLUDED.total,
                items = EXCLUDED.items
            """;
        command.Parameters.AddWithValue("id", order.Id);
        command.Parameters.AddWithValue("status", order.Status.ToString());
        command.Parameters.AddWithValue("total", order.Total);
        command.Parameters.AddWithValue("items", SerializeItems(order.Items));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        return SaveAsync(order, cancellationToken);
    }

    private string SerializeItems(IReadOnlyCollection<OrderItem> items)
    {
        var payload = items.Select(item => new OrderItemPayload(item.Sku, item.Quantity, item.UnitPrice)).ToArray();
        return JsonSerializer.Serialize(payload, jsonOptions);
    }

    private void EnsureTable()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS orders (
                id uuid PRIMARY KEY,
                status text NOT NULL,
                total numeric NOT NULL,
                items jsonb NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    private sealed record OrderItemPayload(string Sku, int Quantity, decimal UnitPrice);
}
