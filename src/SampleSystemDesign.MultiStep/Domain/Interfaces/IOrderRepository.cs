namespace SampleSystemDesign.MultiStep.Domain.Interfaces;

using SampleSystemDesign.MultiStep.Domain.Entities;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}
