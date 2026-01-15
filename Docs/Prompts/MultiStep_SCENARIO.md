# Scenario: ECommerceCheckoutService (Multi-Step)

## Business Goal
Orchestrate the complex checkout process for event tickets, which involves multiple independent services (Payment, Inventory, Notification). The process must be eventually consistent, with compensating actions for failures.

## System Design Pattern: Multi-Step (Saga Pattern)
The solution must implement the **Multi-Step** pattern using the **Saga Pattern** (Orchestration or Choreography, focusing on Orchestration for simplicity):
1.  **Saga Orchestrator:** A central service (simulated via a dedicated handler) that manages the sequence of steps.
2.  **Compensating Transactions:** Logic to undo previous steps if a later step fails (e.g., refund payment if inventory update fails).
3.  **Messaging:** Use of messages to communicate between the orchestrator and the service steps.

## Implementation Tasks (AI Agent Focus)
1.  **Domain:** Define the `Order` Aggregate (Id, Status, Items, Total) and the `IOrderRepository`.
2.  **Application:** Create the `StartCheckoutCommandHandler` which initiates the Saga. Define the interfaces for the external services: `IPaymentService`, `IInventoryService`, `INotificationService`.
3.  **Infrastructure:** Implement the `CheckoutSagaOrchestrator` (a dedicated class) that sends commands and handles responses/failures from the external services.
4.  **Presentation:** Create an API endpoint (`/api/checkout/start`) to begin the process.
