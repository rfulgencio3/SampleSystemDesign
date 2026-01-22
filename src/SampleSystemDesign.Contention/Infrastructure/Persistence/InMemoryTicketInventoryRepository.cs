using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Domain.Interfaces;

namespace SampleSystemDesign.Contention.Infrastructure.Persistence;

public sealed class InMemoryTicketInventoryRepository : ITicketInventoryRepository
{
    private readonly Lock syncRoot = new();
    private readonly Dictionary<Guid, TicketInventory> inventories = [];

    public Task<TicketInventory?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Event ID is required.", nameof(eventId));

        lock (syncRoot)
        {
            inventories.TryGetValue(eventId, out var inventory);
            return Task.FromResult(inventory);
        }
    }

    public Task SaveAsync(TicketInventory inventory, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inventory);

        lock (syncRoot)
        {
            inventories[inventory.EventId] = inventory;
        }

        return Task.CompletedTask;
    }

    public Task<bool> TryUpdateAsync(TicketInventory updatedInventory, int expectedVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updatedInventory);

        var updated = false;

        lock (syncRoot)
        {
            if (inventories.TryGetValue(updatedInventory.EventId, out var current) && current.Version == expectedVersion)
            {
                inventories[updatedInventory.EventId] = updatedInventory;
                updated = true;
            }
        }

        return Task.FromResult(updated);
    }

    public void Seed(IEnumerable<TicketInventory> seedData)
    {
        foreach (var inventory in seedData)
        {
            inventories[inventory.EventId] = inventory;
        }
    }
}
