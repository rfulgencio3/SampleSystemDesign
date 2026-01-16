namespace SampleSystemDesign.MultiStep.Infrastructure.Persistence;

using System.Collections.Concurrent;
using SampleSystemDesign.MultiStep.Domain.Entities;
using SampleSystemDesign.MultiStep.Domain.Interfaces;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> orders = new();

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));

        orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
