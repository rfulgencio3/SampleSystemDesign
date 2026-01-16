namespace SampleSystemDesign.Contention.Infrastructure.Persistence;

using System.Collections.Concurrent;
using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Domain.Interfaces;

public sealed class InMemoryTicketReservationRepository : ITicketReservationRepository
{
    private readonly ConcurrentBag<TicketReservation> reservations = new();

    public Task SaveAsync(TicketReservation reservation, CancellationToken cancellationToken = default)
    {
        if (reservation is null) throw new ArgumentNullException(nameof(reservation));

        reservations.Add(reservation);
        return Task.CompletedTask;
    }

    public int CountForTicket(Guid ticketId)
    {
        return reservations.Count(reservation => reservation.TicketId == ticketId);
    }
}
