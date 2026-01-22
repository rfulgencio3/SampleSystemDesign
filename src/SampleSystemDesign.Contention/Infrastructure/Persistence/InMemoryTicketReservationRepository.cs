using System.Collections.Concurrent;
using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Domain.Interfaces;

namespace SampleSystemDesign.Contention.Infrastructure.Persistence;

public sealed class InMemoryTicketReservationRepository : ITicketReservationRepository
{
    private readonly ConcurrentBag<TicketReservation> reservations = [];

    public Task SaveAsync(TicketReservation reservation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        reservations.Add(reservation);
        return Task.CompletedTask;
    }

    public int CountForTicket(Guid ticketId)
    {
        return reservations.Count(reservation => reservation.TicketId == ticketId);
    }
}
