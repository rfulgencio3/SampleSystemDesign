namespace SampleSystemDesign.Contention.Presentation;

public sealed record ReserveTicketRequest(Guid EventId, string UserId);
