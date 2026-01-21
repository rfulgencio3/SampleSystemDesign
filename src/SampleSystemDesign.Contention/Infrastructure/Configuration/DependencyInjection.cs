namespace SampleSystemDesign.Contention.Infrastructure.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleSystemDesign.Contention.Application.Interfaces;
using SampleSystemDesign.Contention.Application.UseCases;
using SampleSystemDesign.Contention.Domain.Interfaces;
using SampleSystemDesign.Contention.Infrastructure.ExternalServices;
using SampleSystemDesign.Contention.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddContention(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required.");

        var holdMinutes = ParseInt(configuration["Reservation:HoldMinutes"], 5);

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IReservationWindow>(_ => new DefaultReservationWindow(TimeSpan.FromMinutes(holdMinutes)));
        services.AddSingleton<ITicketInventoryRepository>(_ => new PostgresTicketInventoryRepository(connectionString));
        services.AddSingleton<ITicketReservationRepository>(_ => new PostgresTicketReservationRepository(connectionString));
        services.AddSingleton<ReserveTicketCommandHandler>();

        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
