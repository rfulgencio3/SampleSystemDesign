namespace SampleSystemDesign.Contention.Application.DTOs;

public enum ReservationStatus
{
    Reserved,
    SoldOut,
    NotFound,
    Conflict
}

public sealed record ReserveTicketResult(ReservationStatus Status, TicketReservationDto? Reservation)
{
    public static ReserveTicketResult Reserved(TicketReservationDto reservation) =>
        new(ReservationStatus.Reserved, reservation);

    public static ReserveTicketResult SoldOut() => new(ReservationStatus.SoldOut, null);

    public static ReserveTicketResult NotFound() => new(ReservationStatus.NotFound, null);

    public static ReserveTicketResult Conflict() => new(ReservationStatus.Conflict, null);
}
