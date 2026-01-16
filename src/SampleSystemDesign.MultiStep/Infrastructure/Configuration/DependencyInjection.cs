namespace SampleSystemDesign.MultiStep.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.MultiStep.Application.Interfaces;
using SampleSystemDesign.MultiStep.Application.UseCases;
using SampleSystemDesign.MultiStep.Domain.Interfaces;
using SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;
using SampleSystemDesign.MultiStep.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddMultiStep(this IServiceCollection services)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IPaymentService>(_ => new SimulatedPaymentService());
        services.AddSingleton<IInventoryService>(_ => new SimulatedInventoryService());
        services.AddSingleton<INotificationService>(_ => new SimulatedNotificationService());
        services.AddSingleton<ICheckoutMessageBus, InMemoryCheckoutMessageBus>();
        services.AddSingleton<ICheckoutSagaOrchestrator, CheckoutSagaOrchestrator>();
        services.AddSingleton<StartCheckoutCommandHandler>();

        return services;
    }
}
