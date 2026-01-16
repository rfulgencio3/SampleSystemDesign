namespace SampleSystemDesign.MultiStep.Application.Interfaces;

public interface INotificationService
{
    Task<bool> SendOrderConfirmedAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task SendOrderFailedAsync(Guid orderId, string reason, CancellationToken cancellationToken = default);
}
