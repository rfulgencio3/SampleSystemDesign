namespace SampleSystemDesign.Contention.Domain.Entities;

public sealed class TicketReservation
{
    public TicketReservation(Guid ticketId, string userId, DateTimeOffset expirationTime)
    {
        if (ticketId == Guid.Empty) throw new ArgumentException("Ticket ID must not be empty.", nameof(ticketId));
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required.", nameof(userId));

        TicketId = ticketId;
        UserId = userId;
        ExpirationTime = expirationTime;
    }

    public Guid TicketId { get; }
    public string UserId { get; }
    public DateTimeOffset ExpirationTime { get; }

    public bool IsExpired(DateTimeOffset now) => now >= ExpirationTime;
}
