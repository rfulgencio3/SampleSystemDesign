namespace SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;

using SampleSystemDesign.MultiStep.Application.Interfaces;
using SampleSystemDesign.MultiStep.Domain.Entities;

public sealed class SimulatedInventoryService : IInventoryService
{
    private readonly bool shouldReserve;

    public SimulatedInventoryService(bool shouldReserve = true)
    {
        this.shouldReserve = shouldReserve;
    }

    public Task<bool> ReserveAsync(Guid orderId, IReadOnlyCollection<OrderItem> items, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(shouldReserve);
    }

    public Task ReleaseAsync(Guid orderId, IReadOnlyCollection<OrderItem> items, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
