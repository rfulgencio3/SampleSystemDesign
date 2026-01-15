# Scenario: TicketReservationService (Contention)

## Business Goal
Manage the sale of a limited number of high-demand tickets, ensuring that concurrent reservation attempts do not lead to overselling or data corruption.

## System Design Pattern: Contention
The solution must implement the **Contention** pattern using:
1.  **Optimistic Locking:** To detect and prevent concurrent updates to the same ticket inventory record.
2.  **Reservation Logic:** A temporary reservation mechanism to hold a ticket for a short period while the user completes the payment process.

## Implementation Tasks (AI Agent Focus)
1.  **Domain:** Define the `TicketInventory` Entity (Id, EventId, TotalTickets, AvailableTickets, Version) and the `TicketReservation` Entity (TicketId, UserId, ExpirationTime). The `TicketInventory` entity must include a `Version` property for optimistic locking.
2.  **Application:** Create the `ReserveTicketCommandHandler` which attempts to reserve a ticket using the optimistic locking mechanism.
3.  **Infrastructure:** Implement the `ITicketInventoryRepository` using a simulated database context that enforces the optimistic locking check (e.g., checking the version number before update).
4.  **Presentation:** Create an API endpoint (`/api/reserve`) that accepts a reservation request.
