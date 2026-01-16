namespace SampleSystemDesign.Contention.Domain.Interfaces;

using SampleSystemDesign.Contention.Domain.Entities;

public interface ITicketReservationRepository
{
    Task SaveAsync(TicketReservation reservation, CancellationToken cancellationToken = default);
}
