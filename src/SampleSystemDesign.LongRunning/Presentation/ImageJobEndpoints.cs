using SampleSystemDesign.LongRunning.Application.UseCases;
using SampleSystemDesign.LongRunning.Domain.Entities;

namespace SampleSystemDesign.LongRunning.Presentation;

public static class ImageJobEndpoints
{
    public static IEndpointRouteBuilder MapImageJobEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/jobs", async (SubmitImageJobRequest request, SubmitImageJobCommandHandler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new SubmitImageJobCommand(request.OriginalFileUrl), cancellationToken);

            var response = new SubmitImageJobResponse(result.JobId, result.Status.ToString());
            return Results.Accepted($"/api/jobs/{result.JobId}", response);
        });

        endpoints.MapGet("/api/jobs/{jobId:guid}", async (Guid jobId, GetImageJobStatusQueryHandler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetImageJobStatusQuery(jobId), cancellationToken);

            if (!result.Found)
            {
                return Results.NotFound();
            }

            var status = result.Status ?? ImageJobStatus.Pending;
            return Results.Ok(new ImageJobStatusResponse(jobId, status.ToString()!, result.ResultFileUrl));
        });

        return endpoints;
    }
}
