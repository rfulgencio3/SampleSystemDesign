namespace SampleSystemDesign.Contention.Domain.Entities;

public sealed class TicketInventory
{
    public TicketInventory(Guid id, Guid eventId, int totalTickets, int availableTickets, int version)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (eventId == Guid.Empty) throw new ArgumentException("Event ID must not be empty.", nameof(eventId));
        if (totalTickets < 0) throw new ArgumentOutOfRangeException(nameof(totalTickets));
        if (availableTickets < 0 || availableTickets > totalTickets)
        {
            throw new ArgumentOutOfRangeException(nameof(availableTickets));
        }

        if (version < 0) throw new ArgumentOutOfRangeException(nameof(version));

        Id = id;
        EventId = eventId;
        TotalTickets = totalTickets;
        AvailableTickets = availableTickets;
        Version = version;
    }

    public Guid Id { get; }
    public Guid EventId { get; }
    public int TotalTickets { get; }
    public int AvailableTickets { get; }
    public int Version { get; }

    public bool CanReserve => AvailableTickets > 0;

    public TicketInventory ReserveOne()
    {
        if (!CanReserve)
        {
            throw new InvalidOperationException("No tickets available to reserve.");
        }

        return new TicketInventory(Id, EventId, TotalTickets, AvailableTickets - 1, Version + 1);
    }
}
