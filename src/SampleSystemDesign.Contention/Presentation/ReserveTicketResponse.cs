namespace SampleSystemDesign.Contention.Presentation;

public sealed record ReserveTicketResponse(Guid TicketId, string UserId, DateTimeOffset ExpirationTime);
