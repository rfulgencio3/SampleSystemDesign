namespace SampleSystemDesign.Contention.Application.DTOs;

public sealed record TicketReservationDto(Guid TicketId, string UserId, DateTimeOffset ExpirationTime);
