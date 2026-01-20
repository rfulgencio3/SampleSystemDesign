# SampleSystemDesign

## Motivations
- Practice system design patterns with small, focused .NET 10 services.
- Apply Hexagonal Architecture + DDD with clear layer boundaries.
- Keep infrastructure in-memory to highlight patterns and behavior.
- Provide tests as executable examples of each scenario.

## Project Structure
Each project follows the same layered structure:

```
src/
  SampleSystemDesign.<Pattern>/
    Domain/
    Application/
    Infrastructure/
    Presentation/
```

Tests mirror the same structure under `test/`.

## Scenarios and Patterns

| Scenario | System Design Pattern | Project | Stack | Notes |
| --- | --- | --- | --- | --- |
| UrlShortenerService | Scaling Reads | `src/SampleSystemDesign.ScalingReads` | Minimal APIs, in-memory cache (simulated Redis), in-memory repository | Cache-aside read path |
| TelemetryIngestionService | Scaling Writes | `src/SampleSystemDesign.ScalingWrites` | Minimal APIs, Channels queue, BackgroundService, in-memory sharded repository | Producer/consumer + sharding |
| ImageProcessingService | Long-Running Tasks | `src/SampleSystemDesign.LongRunning` | Minimal APIs, Channels queue, BackgroundService, in-memory repository | Job queue + status tracking |
| MarketDataService | Real-Time | `src/SampleSystemDesign.RealTime` | Minimal APIs, SignalR, BackgroundService generator | Live feed via hub |
| AssetStorageService | Large Files | `src/SampleSystemDesign.LargeFiles` | Minimal APIs, simulated storage service, in-memory repository | Presigned URLs + metadata |
| TicketReservationService | Contention | `src/SampleSystemDesign.Contention` | Minimal APIs, in-memory optimistic locking | Reservation window |
| ECommerceCheckoutService | Multi-Step (Saga) | `src/SampleSystemDesign.MultiStep` | Minimal APIs, in-memory saga orchestration | Compensations on failure |

## Stacks by Project
- `src/SampleSystemDesign.ScalingReads`: ASP.NET Core Minimal APIs, in-memory cache (simulated Redis), in-memory repository, xUnit tests.
- `src/SampleSystemDesign.ScalingWrites`: ASP.NET Core Minimal APIs, Channels queue, BackgroundService consumer, in-memory sharded repository, xUnit tests.
- `src/SampleSystemDesign.Contention`: ASP.NET Core Minimal APIs, optimistic locking in-memory repository, reservation window service, xUnit tests.
- `src/SampleSystemDesign.LargeFiles`: ASP.NET Core Minimal APIs, simulated presigned URL storage, in-memory metadata repository, xUnit tests.
- `src/SampleSystemDesign.LongRunning`: ASP.NET Core Minimal APIs, Channels queue, BackgroundService worker, in-memory repository, xUnit tests.
- `src/SampleSystemDesign.MultiStep`: ASP.NET Core Minimal APIs, in-memory saga orchestrator + message bus, simulated external services, xUnit tests.
- `src/SampleSystemDesign.RealTime`: ASP.NET Core Minimal APIs, SignalR hub, BackgroundService generator, xUnit tests.

## Optional docker-compose (future integration)
It is valid to use a single `docker-compose.yml` to provide shared infrastructure (Redis, RabbitMQ/Kafka, Postgres, MinIO, etc.). Today these projects use in-memory adapters, so adopting real services would require swapping Infrastructure implementations and wiring connection strings.

Suggested shared services (one instance each):
- Redis for ScalingReads cache.
- RabbitMQ (or Kafka) for ScalingWrites ingestion queue.
- Postgres (or another RDBMS) for persistence when you move beyond in-memory repositories.
- MinIO (S3-compatible) for LargeFiles object storage.

When using a shared service, keep isolation by:
- Separate queues/topics per project (for example, `telemetry.events` and `checkout.events`).
- Separate Redis key prefixes per project.
- Separate database schemas or databases per project.

High-level flow with a compose stack:
1) `docker compose up -d` starts shared services.
2) Each project reads its connection settings (env vars or appsettings).
3) Run the project and validate the endpoints/flows below.

## Build and Test
Run all tests:

```
dotnet test SampleSystemDesign.sln
```

Run a specific project (example):

```
dotnet run --project src/SampleSystemDesign.ScalingReads
```

## Quick Manual Checks
- ScalingReads: `GET /r/sched` (seeded in `Program.cs`).
- ScalingWrites: `POST /api/telemetry` with JSON payload.
- LongRunning: `POST /api/jobs` then `GET /api/jobs/{id}`.
- RealTime: connect to `/hub/market-data` and listen for `marketData`.
- LargeFiles: `POST /api/assets/upload-url` then `GET /api/assets/{assetId}/download-url`.
- Contention: `POST /api/reserve` with `EventId` and `UserId`.
- MultiStep: `POST /api/checkout/start` with item list.
