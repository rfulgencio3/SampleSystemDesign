using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.Contention.Application.Interfaces;
using SampleSystemDesign.Contention.Application.UseCases;
using SampleSystemDesign.Contention.Domain.Interfaces;
using SampleSystemDesign.Contention.Infrastructure.ExternalServices;
using SampleSystemDesign.Contention.Infrastructure.Persistence;

namespace SampleSystemDesign.Contention.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddContention(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required.");

        var holdMinutes = ParseInt(configuration["Reservation:HoldMinutes"], 5);

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IReservationWindow>(_ => new DefaultReservationWindow(TimeSpan.FromMinutes(holdMinutes)));
        services.AddSingleton<ITicketInventoryRepository>(_ => new DatabaseTicketInventoryRepository(connectionString));
        services.AddSingleton<ITicketReservationRepository>(_ => new DatabaseTicketReservationRepository(connectionString));
        services.AddSingleton<ReserveTicketCommandHandler>();

        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
