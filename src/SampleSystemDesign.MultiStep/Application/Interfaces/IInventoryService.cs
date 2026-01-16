namespace SampleSystemDesign.MultiStep.Application.Interfaces;

using SampleSystemDesign.MultiStep.Domain.Entities;

public interface IInventoryService
{
    Task<bool> ReserveAsync(Guid orderId, IReadOnlyCollection<OrderItem> items, CancellationToken cancellationToken = default);
    Task ReleaseAsync(Guid orderId, IReadOnlyCollection<OrderItem> items, CancellationToken cancellationToken = default);
}
