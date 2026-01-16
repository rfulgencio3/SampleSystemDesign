namespace SampleSystemDesign.MultiStep.Application.UseCases;

using SampleSystemDesign.MultiStep.Application.DTOs;
using SampleSystemDesign.MultiStep.Application.Interfaces;
using SampleSystemDesign.MultiStep.Domain.Entities;
using SampleSystemDesign.MultiStep.Domain.Interfaces;

public sealed class StartCheckoutCommandHandler
{
    private readonly IOrderRepository repository;
    private readonly ICheckoutSagaOrchestrator orchestrator;

    public StartCheckoutCommandHandler(IOrderRepository repository, ICheckoutSagaOrchestrator orchestrator)
    {
        this.repository = repository;
        this.orchestrator = orchestrator;
    }

    public async Task<StartCheckoutResult> HandleAsync(StartCheckoutCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Items is null || command.Items.Count == 0)
        {
            throw new ArgumentException("At least one item is required.", nameof(command));
        }

        var items = command.Items
            .Select(item => new OrderItem(item.Sku, item.Quantity, item.UnitPrice))
            .ToArray();

        var total = items.Sum(item => item.LineTotal);
        var order = new Order(Guid.NewGuid(), OrderStatus.Pending, items, total);

        await repository.SaveAsync(order, cancellationToken);

        var finalOrder = await orchestrator.StartAsync(order, cancellationToken);

        return new StartCheckoutResult(finalOrder.Id, finalOrder.Status, finalOrder.Total);
    }
}
