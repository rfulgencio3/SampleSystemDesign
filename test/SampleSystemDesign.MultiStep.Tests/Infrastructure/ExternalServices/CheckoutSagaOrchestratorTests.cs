using SampleSystemDesign.MultiStep.Application.DTOs;
using SampleSystemDesign.MultiStep.Application.Interfaces;
using SampleSystemDesign.MultiStep.Domain.Entities;
using SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;
using SampleSystemDesign.MultiStep.Infrastructure.Persistence;

namespace SampleSystemDesign.MultiStep.Tests.Infrastructure.ExternalServices;

public class CheckoutSagaOrchestratorTests
{
    [Fact]
    public async Task StartAsync_RefundsPayment_WhenInventoryFails()
    {
        var repository = new InMemoryOrderRepository();
        var payment = new FakePaymentService(true);
        var inventory = new FakeInventoryService(false);
        var notifications = new FakeNotificationService(true);
        var messageBus = new FakeMessageBus();
        var orchestrator = new CheckoutSagaOrchestrator(repository, payment, inventory, notifications, messageBus);

        var items = new[] { new OrderItem("sku-1", 1, 20m) };
        var order = new Order(Guid.NewGuid(), OrderStatus.Pending, items, 20m);
        await repository.SaveAsync(order);

        var result = await orchestrator.StartAsync(order);

        Assert.Equal(OrderStatus.Failed, result.Status);
        Assert.Equal(1, payment.RefundCalls);
        Assert.Equal(0, inventory.ReleaseCalls);
        Assert.Equal(1, notifications.FailedCalls);
    }

    [Fact]
    public async Task StartAsync_ReleasesInventory_WhenNotificationFails()
    {
        var repository = new InMemoryOrderRepository();
        var payment = new FakePaymentService(true);
        var inventory = new FakeInventoryService(true);
        var notifications = new FakeNotificationService(false);
        var messageBus = new FakeMessageBus();
        var orchestrator = new CheckoutSagaOrchestrator(repository, payment, inventory, notifications, messageBus);

        var items = new[] { new OrderItem("sku-1", 1, 12m) };
        var order = new Order(Guid.NewGuid(), OrderStatus.Pending, items, 12m);
        await repository.SaveAsync(order);

        var result = await orchestrator.StartAsync(order);

        Assert.Equal(OrderStatus.Failed, result.Status);
        Assert.Equal(1, payment.RefundCalls);
        Assert.Equal(1, inventory.ReleaseCalls);
        Assert.Equal(1, notifications.FailedCalls);
    }

    private sealed class FakePaymentService(bool shouldSucceed) : IPaymentService
    {
        private readonly bool shouldSucceed = shouldSucceed;

        public int CaptureCalls { get; private set; }
        public int RefundCalls { get; private set; }

        public Task<bool> CapturePaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
        {
            CaptureCalls++;
            return Task.FromResult(shouldSucceed);
        }

        public Task RefundPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
        {
            RefundCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeInventoryService(bool shouldReserve) : IInventoryService
    {
        private readonly bool shouldReserve = shouldReserve;

        public int ReserveCalls { get; private set; }
        public int ReleaseCalls { get; private set; }

        public Task<bool> ReserveAsync(Guid orderId, IReadOnlyCollection<OrderItem> items, CancellationToken cancellationToken = default)
        {
            ReserveCalls++;
            return Task.FromResult(shouldReserve);
        }

        public Task ReleaseAsync(Guid orderId, IReadOnlyCollection<OrderItem> items, CancellationToken cancellationToken = default)
        {
            ReleaseCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeNotificationService(bool shouldConfirm) : INotificationService
    {
        private readonly bool shouldConfirm = shouldConfirm;

        public int ConfirmedCalls { get; private set; }
        public int FailedCalls { get; private set; }

        public Task<bool> SendOrderConfirmedAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            ConfirmedCalls++;
            return Task.FromResult(shouldConfirm);
        }

        public Task SendOrderFailedAsync(Guid orderId, string reason, CancellationToken cancellationToken = default)
        {
            FailedCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeMessageBus : ICheckoutMessageBus
    {
        public List<CheckoutMessage> Messages { get; } = [];

        public Task PublishAsync(CheckoutMessage message, CancellationToken cancellationToken = default)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
