namespace SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;

using SampleSystemDesign.MultiStep.Application.Interfaces;

public sealed class SimulatedNotificationService : INotificationService
{
    private readonly bool shouldConfirm;

    public SimulatedNotificationService(bool shouldConfirm = true)
    {
        this.shouldConfirm = shouldConfirm;
    }

    public Task<bool> SendOrderConfirmedAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(shouldConfirm);
    }

    public Task SendOrderFailedAsync(Guid orderId, string reason, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
