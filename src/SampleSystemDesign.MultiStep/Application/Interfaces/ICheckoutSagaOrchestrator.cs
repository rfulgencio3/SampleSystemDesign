namespace SampleSystemDesign.MultiStep.Application.Interfaces;

using SampleSystemDesign.MultiStep.Domain.Entities;

public interface ICheckoutSagaOrchestrator
{
    Task<Order> StartAsync(Order order, CancellationToken cancellationToken = default);
}
