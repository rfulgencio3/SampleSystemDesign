using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Infrastructure.Persistence;

namespace SampleSystemDesign.Contention.Tests.Infrastructure.Persistence;

public class InMemoryTicketInventoryRepositoryTests
{
    [Fact]
    public async Task TryUpdateAsync_ReturnsFalse_OnOptimisticLockConflict()
    {
        var eventId = Guid.NewGuid();
        var inventory = new TicketInventory(Guid.NewGuid(), eventId, 5, 5, 2);
        var repository = new InMemoryTicketInventoryRepository();
        repository.Seed(new[] { inventory });
        var updated = new TicketInventory(inventory.Id, eventId, 5, 4, 3);

        var result = await repository.TryUpdateAsync(updated, expectedVersion: 1);

        Assert.False(result);
    }
}
