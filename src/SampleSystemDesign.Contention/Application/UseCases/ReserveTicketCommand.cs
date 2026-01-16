namespace SampleSystemDesign.Contention.Application.UseCases;

public sealed record ReserveTicketCommand(Guid EventId, string UserId);
