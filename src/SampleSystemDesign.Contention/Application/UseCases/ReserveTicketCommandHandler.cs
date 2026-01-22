using SampleSystemDesign.Contention.Application.DTOs;
using SampleSystemDesign.Contention.Application.Interfaces;
using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Domain.Interfaces;

namespace SampleSystemDesign.Contention.Application.UseCases;

public sealed class ReserveTicketCommandHandler
{
    private readonly ITicketInventoryRepository inventoryRepository;
    private readonly ITicketReservationRepository reservationRepository;
    private readonly IClock clock;
    private readonly IReservationWindow reservationWindow;
    private readonly int maxRetries;

    public ReserveTicketCommandHandler(
        ITicketInventoryRepository inventoryRepository,
        ITicketReservationRepository reservationRepository,
        IClock clock,
        IReservationWindow reservationWindow,
        int maxRetries = 3)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRetries);

        this.inventoryRepository = inventoryRepository;
        this.reservationRepository = reservationRepository;
        this.clock = clock;
        this.reservationWindow = reservationWindow;
        this.maxRetries = maxRetries;
    }

    public async Task<ReserveTicketResult> HandleAsync(ReserveTicketCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.EventId == Guid.Empty) throw new ArgumentException("Event ID is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.UserId)) throw new ArgumentException("User ID is required.", nameof(command));
        if (reservationWindow.HoldDuration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Reservation window must be greater than zero.");
        }

        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            var inventory = await inventoryRepository.GetByEventIdAsync(command.EventId, cancellationToken);

            if (inventory is null)
            {
                return ReserveTicketResult.NotFound();
            }

            if (!inventory.CanReserve)
            {
                return ReserveTicketResult.SoldOut();
            }

            var updatedInventory = inventory.ReserveOne();

            var updated = await inventoryRepository.TryUpdateAsync(updatedInventory, inventory.Version, cancellationToken);

            if (!updated)
            {
                continue;
            }

            var expirationTime = clock.UtcNow.Add(reservationWindow.HoldDuration);
            var reservation = new TicketReservation(updatedInventory.Id, command.UserId, expirationTime);

            await reservationRepository.SaveAsync(reservation, cancellationToken);

            return ReserveTicketResult.Reserved(new TicketReservationDto(reservation.TicketId, reservation.UserId, reservation.ExpirationTime));
        }

        return ReserveTicketResult.Conflict();
    }
}
