namespace SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;

using SampleSystemDesign.MultiStep.Application.Interfaces;

public sealed class SimulatedPaymentService : IPaymentService
{
    private readonly bool shouldSucceed;

    public SimulatedPaymentService(bool shouldSucceed = true)
    {
        this.shouldSucceed = shouldSucceed;
    }

    public Task<bool> CapturePaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(shouldSucceed);
    }

    public Task RefundPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
