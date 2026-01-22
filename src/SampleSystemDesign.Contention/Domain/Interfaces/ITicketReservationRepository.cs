using SampleSystemDesign.Contention.Domain.Entities;

namespace SampleSystemDesign.Contention.Domain.Interfaces;

public interface ITicketReservationRepository
{
    Task SaveAsync(TicketReservation reservation, CancellationToken cancellationToken = default);
}
