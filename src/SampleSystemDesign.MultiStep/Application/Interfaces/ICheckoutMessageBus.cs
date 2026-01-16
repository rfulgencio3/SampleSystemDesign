namespace SampleSystemDesign.MultiStep.Application.Interfaces;

using SampleSystemDesign.MultiStep.Application.DTOs;

public interface ICheckoutMessageBus
{
    Task PublishAsync(CheckoutMessage message, CancellationToken cancellationToken = default);
}
