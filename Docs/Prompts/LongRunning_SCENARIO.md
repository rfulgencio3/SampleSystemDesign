# Scenario: ImageProcessingService (Long-Running Tasks)

## Business Goal
Implement a service to process and resize high-resolution images uploaded by event organizers. This is a CPU-intensive task that should not block the user's web request.

## System Design Pattern: Long-Running Tasks
The solution must implement the **Long-Running Tasks** pattern using:
1.  **Asynchronous Worker:** A dedicated background service (`IHostedService`) to perform the heavy lifting.
2.  **Job Queue:** A simple in-memory or lightweight queue (e.g., Hangfire or a database table) to manage job requests.
3.  **Status Tracking:** The service must provide a way for the user to check the status of the processing job.

## Implementation Tasks (AI Agent Focus)
1.  **Domain:** Define the `ImageJob` Entity (Id, OriginalFileUrl, Status, ResultFileUrl) and the `IImageJobRepository`.
2.  **Application:** Create the `SubmitImageJobCommandHandler` which creates the job record and enqueues the request.
3.  **Infrastructure:** Implement the `ImageProcessingWorker` (IHostedService) that consumes the queue, performs a simulated long-running process, and updates the job status.
4.  **Presentation:** Create API endpoints for submitting a job (`/api/jobs`) and checking its status (`/api/jobs/{id}`).
