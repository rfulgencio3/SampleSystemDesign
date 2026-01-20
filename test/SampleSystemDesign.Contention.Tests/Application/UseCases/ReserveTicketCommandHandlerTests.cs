using SampleSystemDesign.Contention.Application.DTOs;
using SampleSystemDesign.Contention.Application.Interfaces;
using SampleSystemDesign.Contention.Application.UseCases;
using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Infrastructure.ExternalServices;
using SampleSystemDesign.Contention.Infrastructure.Persistence;

namespace SampleSystemDesign.Contention.Tests.Application.UseCases;

public class ReserveTicketCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReservesTicket_WhenAvailable()
    {
        var eventId = Guid.NewGuid();
        var inventory = new TicketInventory(Guid.NewGuid(), eventId, 10, 10, 0);
        var inventoryRepository = new InMemoryTicketInventoryRepository();
        inventoryRepository.Seed(new[] { inventory });
        var reservationRepository = new InMemoryTicketReservationRepository();
        var clock = new FakeClock(DateTimeOffset.UtcNow);
        var window = new DefaultReservationWindow(TimeSpan.FromMinutes(2));
        var handler = new ReserveTicketCommandHandler(inventoryRepository, reservationRepository, clock, window);

        var result = await handler.HandleAsync(new ReserveTicketCommand(eventId, "user-1"));
        var updated = await inventoryRepository.GetByEventIdAsync(eventId);

        Assert.Equal(ReservationStatus.Reserved, result.Status);
        Assert.NotNull(result.Reservation);
        Assert.NotNull(updated);
        Assert.Equal(9, updated!.AvailableTickets);
        Assert.Equal(1, updated.Version);
        Assert.Equal(clock.UtcNow.Add(window.HoldDuration), result.Reservation!.ExpirationTime);
    }

    [Fact]
    public async Task HandleAsync_ReturnsSoldOut_WhenInventoryIsEmpty()
    {
        var eventId = Guid.NewGuid();
        var inventory = new TicketInventory(Guid.NewGuid(), eventId, 1, 0, 0);
        var inventoryRepository = new InMemoryTicketInventoryRepository();
        inventoryRepository.Seed(new[] { inventory });
        var reservationRepository = new InMemoryTicketReservationRepository();
        var handler = new ReserveTicketCommandHandler(
            inventoryRepository,
            reservationRepository,
            new FakeClock(DateTimeOffset.UtcNow),
            new DefaultReservationWindow(TimeSpan.FromMinutes(1)));

        var result = await handler.HandleAsync(new ReserveTicketCommand(eventId, "user-2"));

        Assert.Equal(ReservationStatus.SoldOut, result.Status);
        Assert.Null(result.Reservation);
    }

    private sealed class FakeClock : IClock
    {
        public FakeClock(DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTimeOffset UtcNow { get; }
    }
}
