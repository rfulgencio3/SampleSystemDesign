namespace SampleSystemDesign.ScalingWrites.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SampleSystemDesign.ScalingWrites.Application.UseCases;

public static class TelemetryEndpoints
{
    public static IEndpointRouteBuilder MapTelemetryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/telemetry", async (TelemetryIngestRequest request, IngestTelemetryCommandHandler handler, CancellationToken cancellationToken) =>
        {
            var command = new IngestTelemetryCommand(request.DeviceId, request.MetricName, request.Value, request.Timestamp);
            await handler.HandleAsync(command, cancellationToken);
            return Results.Accepted();
        });

        return endpoints;
    }
}
