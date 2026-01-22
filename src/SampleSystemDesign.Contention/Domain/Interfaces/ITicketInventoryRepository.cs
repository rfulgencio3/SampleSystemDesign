using SampleSystemDesign.Contention.Domain.Entities;

namespace SampleSystemDesign.Contention.Domain.Interfaces;

public interface ITicketInventoryRepository
{
    Task<TicketInventory?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task SaveAsync(TicketInventory inventory, CancellationToken cancellationToken = default);
    Task<bool> TryUpdateAsync(TicketInventory updatedInventory, int expectedVersion, CancellationToken cancellationToken = default);
}
