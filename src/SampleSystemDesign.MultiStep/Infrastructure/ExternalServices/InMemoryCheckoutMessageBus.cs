namespace SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;

using System.Collections.Concurrent;
using SampleSystemDesign.MultiStep.Application.DTOs;
using SampleSystemDesign.MultiStep.Application.Interfaces;

public sealed class InMemoryCheckoutMessageBus : ICheckoutMessageBus
{
    private readonly ConcurrentQueue<CheckoutMessage> messages = new();

    public Task PublishAsync(CheckoutMessage message, CancellationToken cancellationToken = default)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));

        messages.Enqueue(message);
        return Task.CompletedTask;
    }

    public IReadOnlyCollection<CheckoutMessage> GetAll() => messages.ToArray();
}
