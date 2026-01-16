namespace SampleSystemDesign.Contention.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SampleSystemDesign.Contention.Application.DTOs;
using SampleSystemDesign.Contention.Application.UseCases;

public static class TicketReservationEndpoints
{
    public static IEndpointRouteBuilder MapTicketReservationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/reserve", async (ReserveTicketRequest request, ReserveTicketCommandHandler handler, CancellationToken cancellationToken) =>
        {
            var command = new ReserveTicketCommand(request.EventId, request.UserId);
            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Status switch
            {
                ReservationStatus.Reserved => Results.Created($"/api/reserve/{result.Reservation!.TicketId}",
                    new ReserveTicketResponse(result.Reservation.TicketId, result.Reservation.UserId, result.Reservation.ExpirationTime)),
                ReservationStatus.NotFound => Results.NotFound(),
                ReservationStatus.SoldOut => Results.Conflict(new { error = "Tickets are sold out." }),
                ReservationStatus.Conflict => Results.Conflict(new { error = "Reservation conflict. Please retry." }),
                _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
            };
        });

        return endpoints;
    }
}
