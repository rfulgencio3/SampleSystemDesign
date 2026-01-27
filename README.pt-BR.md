# SampleSystemDesign
[English](README.md) | [Português (pt-BR)](README.pt-BR.md)

## Motivações
- Praticar padrões de system design com serviços .NET 10 pequenos e focados.
- Aplicar Arquitetura Hexagonal + DDD com limites claros entre camadas.
- Fornecer adapters de infraestrutura reais com versões in-memory para testes e demos rápidas.
- Oferecer testes como exemplos executáveis de cada cenário.

## Estrutura do Projeto
Cada projeto segue a mesma estrutura em camadas:

```
src/
  SampleSystemDesign.<Pattern>/
    Domain/
    Application/
    Infrastructure/
    Presentation/
```

Os testes espelham a mesma estrutura em `test/`.

## Cenários e Padrões

| Cenário | Padrão de System Design | Projeto | Stack | Observações |
| --- | --- | --- | --- | --- |
| UrlShortenerService | Scaling Reads | `src/SampleSystemDesign.ScalingReads` | Minimal APIs, Redis cache-aside, repositório Postgres (adapters in-memory para testes) | Leitura cache-aside |
| TelemetryIngestionService | Scaling Writes | `src/SampleSystemDesign.ScalingWrites` | Minimal APIs, fila RabbitMQ, BackgroundService, repositório Postgres shardado (in-memory para testes) | Producer/consumer + sharding |
| ImageProcessingService | Long-Running Tasks | `src/SampleSystemDesign.LongRunning` | Minimal APIs, fila RabbitMQ, BackgroundService, repositório Postgres (in-memory para testes) | Job queue + status |
| MarketDataService | Real-Time | `src/SampleSystemDesign.RealTime` | Minimal APIs, SignalR, gerador em BackgroundService | Feed ao vivo via hub |
| AssetStorageService | Large Files | `src/SampleSystemDesign.LargeFiles` | Minimal APIs, storage MinIO, metadados em Postgres (in-memory para testes) | Presigned URLs + metadados |
| TicketReservationService | Contention | `src/SampleSystemDesign.Contention` | Minimal APIs, optimistic locking em Postgres (in-memory para testes) | Janela de reserva |
| ECommerceCheckoutService | Multi-Step (Saga) | `src/SampleSystemDesign.MultiStep` | Minimal APIs, bus RabbitMQ, pedidos em Postgres, serviços externos simulados (in-memory para testes) | Compensações em falha |

## Stacks por Projeto
- `src/SampleSystemDesign.ScalingReads`: ASP.NET Core Minimal APIs, Redis cache-aside, repositório Postgres, testes xUnit (adapters in-memory nos testes).
- `src/SampleSystemDesign.ScalingWrites`: ASP.NET Core Minimal APIs, fila RabbitMQ, BackgroundService consumidor, repositório Postgres shardado, testes xUnit (adapters in-memory nos testes).
- `src/SampleSystemDesign.Contention`: ASP.NET Core Minimal APIs, repositórios Postgres com optimistic locking, serviço de janela de reserva, testes xUnit (adapters in-memory nos testes).
- `src/SampleSystemDesign.LargeFiles`: ASP.NET Core Minimal APIs, storage MinIO, repositório de metadados em Postgres, testes xUnit (adapters in-memory nos testes).
- `src/SampleSystemDesign.LongRunning`: ASP.NET Core Minimal APIs, fila RabbitMQ, BackgroundService worker, repositório Postgres, testes xUnit (adapters in-memory nos testes).
- `src/SampleSystemDesign.MultiStep`: ASP.NET Core Minimal APIs, bus RabbitMQ, pedidos em Postgres, serviços externos simulados, testes xUnit (adapters in-memory nos testes).
- `src/SampleSystemDesign.RealTime`: ASP.NET Core Minimal APIs, hub SignalR, gerador em BackgroundService, testes xUnit.

## Docker Compose (infra local)
Use `docker compose up -d` para iniciar os serviços compartilhados usados pelos adapters de infraestrutura.

Serviços compartilhados sugeridos (uma instancia cada):
- Redis para cache do ScalingReads (porta 6379).
- RabbitMQ para filas do ScalingWrites e MultiStep (porta 5672 AMQP, UI em `http://localhost:15672` com `guest/guest`).
- Postgres para persistencia (porta 5432, db `sample_system_design`, user `sample`, senha `sample`).
- MinIO para storage do LargeFiles (porta 9000 API, UI em `http://localhost:9001` com `minio/minio123`).

Ao usar um serviço compartilhado, mantenha o isolamento por:
- Filas/topicos separados por projeto (ex.: `telemetry.events` e `checkout.events`).
- Prefixos de chave distintos no Redis.
- Schemas ou databases separados por projeto.

Fluxo em alto nivel com compose:
1) `docker compose up -d` inicia os serviços.
2) Cada projeto le suas configuracoes (env vars ou appsettings).
3) Rode o projeto e valide os endpoints/fluxos abaixo.

## Provisionamento de Recursos
O `docker-compose.yml` apenas inicia os serviços de infraestrutura. Cada aplicacao provisiona seus recursos no startup:
- Filas do RabbitMQ sao declaradas pelos servicos (sem exchanges ou topics customizados).
- Tabelas do Postgres sao criadas pelos repositorios.
- Buckets do MinIO sao criados sob demanda pelo storage.

## Portas Locais (sugestao)
Se voce rodar varios servicos ao mesmo tempo, use portas explicitas:
- ScalingReads: `http://localhost:5101`
- ScalingWrites: `http://localhost:5102`
- Contention: `http://localhost:5103`
- LargeFiles: `http://localhost:5104`
- LongRunning: `http://localhost:5105`
- MultiStep: `http://localhost:5106`
- RealTime: `http://localhost:5107`

## Build e Testes
Rodar todos os testes:

```
dotnet test SampleSystemDesign.sln
```

Rodar um projeto especifico (exemplo):

```
dotnet run --project src/SampleSystemDesign.ScalingReads -- --urls http://localhost:5101
```

## Swagger
Todas as APIs HTTP expoem Swagger UI em `/swagger` quando `ASPNETCORE_ENVIRONMENT=Development` (padrao local).
Exemplo: `http://localhost:5101/swagger`.
Os profiles de launch (`launchSettings.json`) definem `launchUrl` como `swagger` para abrir o navegador automaticamente.

## Verificacoes Rapidas
- ScalingReads: `GET http://localhost:5101/r/sched` (seed em `Program.cs`).
- ScalingWrites: `POST http://localhost:5102/api/telemetry`.
- LongRunning: `POST http://localhost:5105/api/jobs` e `GET http://localhost:5105/api/jobs/{id}`.
- RealTime: conecte em `http://localhost:5107/hub/market-data` e escute `marketData`.
- LargeFiles: `POST http://localhost:5104/api/assets/upload-url` e `GET http://localhost:5104/api/assets/{assetId}/download-url`.
- Contention: `POST http://localhost:5103/api/reserve` com `EventId` e `UserId`.
- MultiStep: `POST http://localhost:5106/api/checkout/start` com lista de items.

## Variaveis de Ambiente por Projeto
Todos os projetos leem `appsettings.json`, mas voce pode sobrescrever com variaveis de ambiente.

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

## Scripts de Teste
Voce pode usar estes scripts apos iniciar a infraestrutura compartilhada com `docker compose up -d`.

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
