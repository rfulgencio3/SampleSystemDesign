namespace SampleSystemDesign.MultiStep.Application.Interfaces;

public interface IPaymentService
{
    Task<bool> CapturePaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
    Task RefundPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
}
