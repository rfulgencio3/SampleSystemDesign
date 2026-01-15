# Scenario: MarketDataService (Real-Time)

## Business Goal
Provide a real-time feed of market data (simulated stock prices/ticket prices) to a trading dashboard for immediate decision-making.

## System Design Pattern: Real-Time
The solution must implement the **Real-Time** pattern using:
1.  **SignalR:** To establish persistent, bi-directional connections with clients (the dashboard).
2.  **Hubs:** A SignalR Hub to manage connections and broadcast data.
3.  **Data Generator:** A background service that simulates data changes and pushes them to the Hub.

## Implementation Tasks (AI Agent Focus)
1.  **Domain:** Define the `MarketData` Value Object (Symbol, Price, Change).
2.  **Application:** Create the `IMarketDataPublisher` interface for pushing data.
3.  **Infrastructure:** Implement the `MarketDataHub` (SignalR Hub) and the `MarketDataGeneratorService` (IHostedService) that periodically generates new data and uses the Hub to broadcast it to all connected clients.
4.  **Presentation:** Configure the ASP.NET Core pipeline to use SignalR and define the Hub route.
