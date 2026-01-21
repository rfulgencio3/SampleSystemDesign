namespace SampleSystemDesign.MultiStep.Infrastructure.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.MultiStep.Application.Interfaces;
using SampleSystemDesign.MultiStep.Application.UseCases;
using SampleSystemDesign.MultiStep.Domain.Interfaces;
using SampleSystemDesign.MultiStep.Infrastructure.ExternalServices;
using SampleSystemDesign.MultiStep.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddMultiStep(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required.");

        var host = configuration["RabbitMq:HostName"] ?? "localhost";
        var port = ParseInt(configuration["RabbitMq:Port"], 5672);
        var user = configuration["RabbitMq:UserName"] ?? "guest";
        var password = configuration["RabbitMq:Password"] ?? "guest";
        var queue = configuration["RabbitMq:Queue"] ?? "checkout.events";

        services.AddSingleton<IOrderRepository>(_ => new PostgresOrderRepository(connectionString));
        services.AddSingleton<IPaymentService>(_ => new SimulatedPaymentService());
        services.AddSingleton<IInventoryService>(_ => new SimulatedInventoryService());
        services.AddSingleton<INotificationService>(_ => new SimulatedNotificationService());
        services.AddSingleton<ICheckoutMessageBus>(_ => new RabbitMqCheckoutMessageBus(host, port, user, password, queue));
        services.AddSingleton<ICheckoutSagaOrchestrator, CheckoutSagaOrchestrator>();
        services.AddSingleton<StartCheckoutCommandHandler>();

        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
