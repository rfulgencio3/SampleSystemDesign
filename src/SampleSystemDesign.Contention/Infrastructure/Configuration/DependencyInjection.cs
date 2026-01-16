namespace SampleSystemDesign.Contention.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.Contention.Application.Interfaces;
using SampleSystemDesign.Contention.Application.UseCases;
using SampleSystemDesign.Contention.Domain.Interfaces;
using SampleSystemDesign.Contention.Infrastructure.ExternalServices;
using SampleSystemDesign.Contention.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddContention(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IReservationWindow>(_ => new DefaultReservationWindow(TimeSpan.FromMinutes(5)));
        services.AddSingleton<InMemoryTicketInventoryRepository>();
        services.AddSingleton<ITicketInventoryRepository>(sp => sp.GetRequiredService<InMemoryTicketInventoryRepository>());
        services.AddSingleton<InMemoryTicketReservationRepository>();
        services.AddSingleton<ITicketReservationRepository>(sp => sp.GetRequiredService<InMemoryTicketReservationRepository>());
        services.AddSingleton<ReserveTicketCommandHandler>();

        return services;
    }
}
