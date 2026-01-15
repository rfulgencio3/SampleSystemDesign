# Scenario: TelemetryIngestionService (Scaling Writes)

## Business Goal
Develop a robust ingestion service for telemetry data from thousands of IoT devices at event venues. The service must handle high-volume, bursty write traffic (up to 10,000 events/second) without service degradation.

## System Design Pattern: Scaling Writes
The solution must implement the **Scaling Writes** pattern using:
1.  **Message Queue (Kafka/RabbitMQ):** To decouple the ingestion API from the persistence layer, acting as a buffer for write bursts.
2.  **Asynchronous Processing:** A dedicated worker will consume messages from the queue.
3.  **Sharding:** The persistence layer (simulated) will be designed to distribute data based on a key (e.g., Device ID) to handle high write throughput.

## Implementation Tasks (AI Agent Focus)
1.  **Domain:** Define the `TelemetryEvent` Entity (DeviceId, Timestamp, MetricName, Value) and the `ITelemetryRepository` interface.
2.  **Application:** Create the `IngestTelemetryCommandHandler` which validates the input and publishes the event to the message queue.
3.  **Infrastructure:** Implement a `TelemetryQueueProducer` and a `TelemetryQueueConsumer` (BackgroundService) that handles batching and persistence to the sharded database.
4.  **Presentation:** Create a minimal API endpoint (`/api/telemetry`) to receive the raw data.
