namespace SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;

using SampleSystemDesign.MultiStep.Application.DTOs;
using SampleSystemDesign.MultiStep.Application.Interfaces;
using SampleSystemDesign.MultiStep.Domain.Entities;
using SampleSystemDesign.MultiStep.Domain.Interfaces;

public sealed class CheckoutSagaOrchestrator : ICheckoutSagaOrchestrator
{
    private readonly IOrderRepository repository;
    private readonly IPaymentService paymentService;
    private readonly IInventoryService inventoryService;
    private readonly INotificationService notificationService;
    private readonly ICheckoutMessageBus messageBus;

    public CheckoutSagaOrchestrator(
        IOrderRepository repository,
        IPaymentService paymentService,
        IInventoryService inventoryService,
        INotificationService notificationService,
        ICheckoutMessageBus messageBus)
    {
        this.repository = repository;
        this.paymentService = paymentService;
        this.inventoryService = inventoryService;
        this.notificationService = notificationService;
        this.messageBus = messageBus;
    }

    public async Task<Order> StartAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));

        await PublishAsync(order.Id, "Order", "Created", null, cancellationToken);

        await PublishAsync(order.Id, "Payment", "Requested", null, cancellationToken);
        var paymentOk = await paymentService.CapturePaymentAsync(order.Id, order.Total, cancellationToken);
        await PublishAsync(order.Id, "Payment", paymentOk ? "Succeeded" : "Failed", null, cancellationToken);

        if (!paymentOk)
        {
            return await FailAsync(order, "PaymentFailed", cancellationToken);
        }

        var current = await UpdateStatusAsync(order, OrderStatus.PaymentAuthorized, cancellationToken);

        await PublishAsync(order.Id, "Inventory", "Requested", null, cancellationToken);
        var inventoryOk = await inventoryService.ReserveAsync(order.Id, current.Items, cancellationToken);
        await PublishAsync(order.Id, "Inventory", inventoryOk ? "Succeeded" : "Failed", null, cancellationToken);

        if (!inventoryOk)
        {
            await paymentService.RefundPaymentAsync(order.Id, current.Total, cancellationToken);
            await PublishAsync(order.Id, "Payment", "Refunded", null, cancellationToken);

            return await FailAsync(current, "InventoryFailed", cancellationToken);
        }

        current = await UpdateStatusAsync(current, OrderStatus.InventoryReserved, cancellationToken);

        await PublishAsync(order.Id, "Notification", "Requested", null, cancellationToken);
        var notificationOk = await notificationService.SendOrderConfirmedAsync(order.Id, cancellationToken);
        await PublishAsync(order.Id, "Notification", notificationOk ? "Succeeded" : "Failed", null, cancellationToken);

        if (!notificationOk)
        {
            await inventoryService.ReleaseAsync(order.Id, current.Items, cancellationToken);
            await PublishAsync(order.Id, "Compensation", "InventoryReleased", null, cancellationToken);

            await paymentService.RefundPaymentAsync(order.Id, current.Total, cancellationToken);
            await PublishAsync(order.Id, "Compensation", "PaymentRefunded", null, cancellationToken);

            return await FailAsync(current, "NotificationFailed", cancellationToken);
        }

        var completed = await UpdateStatusAsync(current, OrderStatus.Completed, cancellationToken);
        await PublishAsync(order.Id, "Order", "Completed", null, cancellationToken);

        return completed;
    }

    private async Task<Order> UpdateStatusAsync(Order order, OrderStatus status, CancellationToken cancellationToken)
    {
        var updated = order.WithStatus(status);
        await repository.UpdateAsync(updated, cancellationToken);
        return updated;
    }

    private async Task<Order> FailAsync(Order order, string reason, CancellationToken cancellationToken)
    {
        var failed = order.WithStatus(OrderStatus.Failed);
        await repository.UpdateAsync(failed, cancellationToken);
        await PublishAsync(order.Id, "Order", "Failed", reason, cancellationToken);
        await notificationService.SendOrderFailedAsync(order.Id, reason, cancellationToken);
        return failed;
    }

    private Task PublishAsync(Guid orderId, string step, string status, string? detail, CancellationToken cancellationToken)
    {
        return messageBus.PublishAsync(new CheckoutMessage(orderId, step, status, detail), cancellationToken);
    }
}
