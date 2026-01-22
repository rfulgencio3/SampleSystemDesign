# SampleSystemDesign

## Motivations
- Practice system design patterns with small, focused .NET 10 services.
- Apply Hexagonal Architecture + DDD with clear layer boundaries.
- Provide real infrastructure adapters with in-memory versions for fast tests and demos.
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
| UrlShortenerService | Scaling Reads | `src/SampleSystemDesign.ScalingReads` | Minimal APIs, Redis cache-aside, Postgres repository (in-memory adapters for tests) | Cache-aside read path |
| TelemetryIngestionService | Scaling Writes | `src/SampleSystemDesign.ScalingWrites` | Minimal APIs, RabbitMQ queue, BackgroundService, Postgres sharded repository (in-memory for tests) | Producer/consumer + sharding |
| ImageProcessingService | Long-Running Tasks | `src/SampleSystemDesign.LongRunning` | Minimal APIs, RabbitMQ queue, BackgroundService, Postgres repository (in-memory for tests) | Job queue + status tracking |
| MarketDataService | Real-Time | `src/SampleSystemDesign.RealTime` | Minimal APIs, SignalR, BackgroundService generator | Live feed via hub |
| AssetStorageService | Large Files | `src/SampleSystemDesign.LargeFiles` | Minimal APIs, MinIO storage, Postgres metadata (in-memory for tests) | Presigned URLs + metadata |
| TicketReservationService | Contention | `src/SampleSystemDesign.Contention` | Minimal APIs, Postgres optimistic locking (in-memory for tests) | Reservation window |
| ECommerceCheckoutService | Multi-Step (Saga) | `src/SampleSystemDesign.MultiStep` | Minimal APIs, RabbitMQ bus, Postgres orders, simulated external services (in-memory for tests) | Compensations on failure |

## Stacks by Project
- `src/SampleSystemDesign.ScalingReads`: ASP.NET Core Minimal APIs, Redis cache-aside, Postgres repository, xUnit tests (in-memory adapters in tests).
- `src/SampleSystemDesign.ScalingWrites`: ASP.NET Core Minimal APIs, RabbitMQ queue, BackgroundService consumer, Postgres sharded repository, xUnit tests (in-memory adapters in tests).
- `src/SampleSystemDesign.Contention`: ASP.NET Core Minimal APIs, Postgres optimistic locking repositories, reservation window service, xUnit tests (in-memory adapters in tests).
- `src/SampleSystemDesign.LargeFiles`: ASP.NET Core Minimal APIs, MinIO storage, Postgres metadata repository, xUnit tests (in-memory adapters in tests).
- `src/SampleSystemDesign.LongRunning`: ASP.NET Core Minimal APIs, RabbitMQ queue, BackgroundService worker, Postgres repository, xUnit tests (in-memory adapters in tests).
- `src/SampleSystemDesign.MultiStep`: ASP.NET Core Minimal APIs, RabbitMQ message bus, Postgres orders, simulated external services, xUnit tests (in-memory adapters in tests).
- `src/SampleSystemDesign.RealTime`: ASP.NET Core Minimal APIs, SignalR hub, BackgroundService generator, xUnit tests.

## Docker Compose (local infra)
Use `docker compose up -d` to start the shared services used by the infrastructure adapters.

Suggested shared services (one instance each):
- Redis for ScalingReads cache (port 6379).
- RabbitMQ for ScalingWrites and MultiStep queues (ports 5672 AMQP, 15672 UI at `http://localhost:15672` with `guest/guest`).
- Postgres for persistence (port 5432, db `sample_system_design`, user `sample`, password `sample`).
- MinIO for LargeFiles object storage (port 9000 API, 9001 UI at `http://localhost:9001` with `minio/minio123`).

When using a shared service, keep isolation by:
- Separate queues/topics per project (for example, `telemetry.events` and `checkout.events`).
- Separate Redis key prefixes per project.
- Separate database schemas or databases per project.

High-level flow with a compose stack:
1) `docker compose up -d` starts shared services.
2) Each project reads its connection settings (env vars or appsettings).
3) Run the project and validate the endpoints/flows below.

## Local Ports (suggested)
If you run multiple services at once, use explicit ports:
- ScalingReads: `http://localhost:5101`
- ScalingWrites: `http://localhost:5102`
- Contention: `http://localhost:5103`
- LargeFiles: `http://localhost:5104`
- LongRunning: `http://localhost:5105`
- MultiStep: `http://localhost:5106`
- RealTime: `http://localhost:5107`

## Build and Test
Run all tests:

```
dotnet test SampleSystemDesign.sln
```

Run a specific project (example):

```
dotnet run --project src/SampleSystemDesign.ScalingReads -- --urls http://localhost:5101
```

## Swagger
All HTTP APIs expose Swagger UI at `/swagger` when `ASPNETCORE_ENVIRONMENT=Development` (default for local runs).
Example: `http://localhost:5101/swagger`.

## Quick Manual Checks
- ScalingReads: `GET http://localhost:5101/r/sched` (seeded in `Program.cs`).
- ScalingWrites: `POST http://localhost:5102/api/telemetry`.
- LongRunning: `POST http://localhost:5105/api/jobs` then `GET http://localhost:5105/api/jobs/{id}`.
- RealTime: connect to `http://localhost:5107/hub/market-data` and listen for `marketData`.
- LargeFiles: `POST http://localhost:5104/api/assets/upload-url` then `GET http://localhost:5104/api/assets/{assetId}/download-url`.
- Contention: `POST http://localhost:5103/api/reserve` with `EventId` and `UserId`.
- MultiStep: `POST http://localhost:5106/api/checkout/start` with item list.

## Environment Variables by Project
All projects read from `appsettings.json`, but you can override with environment variables.

### ScalingReads (`src/SampleSystemDesign.ScalingReads`)
- `ConnectionStrings__Postgres`
- `Redis__ConnectionString`
- `Redis__InstanceName`
- `Caching__DefaultTtlMinutes`

### ScalingWrites (`src/SampleSystemDesign.ScalingWrites`)
- `ConnectionStrings__Postgres`
- `RabbitMq__HostName`
- `RabbitMq__Port`
- `RabbitMq__UserName`
- `RabbitMq__Password`
- `RabbitMq__Queue`
- `Telemetry__ShardCount`
- `Telemetry__BatchSize`

### Contention (`src/SampleSystemDesign.Contention`)
- `ConnectionStrings__Postgres`
- `Reservation__HoldMinutes`

### LargeFiles (`src/SampleSystemDesign.LargeFiles`)
- `ConnectionStrings__Postgres`
- `Minio__Endpoint`
- `Minio__AccessKey`
- `Minio__SecretKey`
- `Minio__Bucket`
- `Minio__Secure`
- `Storage__UrlTtlMinutes`

### LongRunning (`src/SampleSystemDesign.LongRunning`)
- `ConnectionStrings__Postgres`
- `RabbitMq__HostName`
- `RabbitMq__Port`
- `RabbitMq__UserName`
- `RabbitMq__Password`
- `RabbitMq__Queue`
- `Processing__DelaySeconds`

### MultiStep (`src/SampleSystemDesign.MultiStep`)
- `ConnectionStrings__Postgres`
- `RabbitMq__HostName`
- `RabbitMq__Port`
- `RabbitMq__UserName`
- `RabbitMq__Password`
- `RabbitMq__Queue`

### RealTime (`src/SampleSystemDesign.RealTime`)
- `MarketData__IntervalSeconds`

## Test Scripts
You can use these scripts after starting the shared infrastructure with `docker compose up -d`.

### ScalingReads
```bash
curl http://localhost:5101/r/sched
```

### ScalingWrites
```bash
curl -X POST http://localhost:5102/api/telemetry \
  -H "Content-Type: application/json" \
  -d "{\"deviceId\":\"device-1\",\"metricName\":\"temp\",\"value\":21.5}"
```

### Contention
```bash
curl -X POST http://localhost:5103/api/reserve \
  -H "Content-Type: application/json" \
  -d "{\"eventId\":\"11111111-1111-1111-1111-111111111111\",\"userId\":\"user-1\"}"
```

### LargeFiles
```bash
curl -X POST http://localhost:5104/api/assets/upload-url \
  -H "Content-Type: application/json" \
  -d "{\"fileName\":\"photo.jpg\",\"contentType\":\"image/jpeg\",\"uploadedBy\":\"user-1\"}"
```

### LongRunning
```bash
curl -X POST http://localhost:5105/api/jobs \
  -H "Content-Type: application/json" \
  -d "{\"originalFileUrl\":\"https://files.example.com/image.jpg\"}"
```

### MultiStep
```bash
curl -X POST http://localhost:5106/api/checkout/start \
  -H "Content-Type: application/json" \
  -d "{\"items\":[{\"sku\":\"sku-1\",\"quantity\":2,\"unitPrice\":10},{\"sku\":\"sku-2\",\"quantity\":1,\"unitPrice\":5}]}"
```
