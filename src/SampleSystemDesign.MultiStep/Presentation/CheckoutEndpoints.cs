namespace SampleSystemDesign.MultiStep.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SampleSystemDesign.MultiStep.Application.DTOs;
using SampleSystemDesign.MultiStep.Application.UseCases;

public static class CheckoutEndpoints
{
    public static IEndpointRouteBuilder MapCheckoutEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/checkout/start", async (StartCheckoutRequest request, StartCheckoutCommandHandler handler, CancellationToken cancellationToken) =>
        {
            if (request.Items is null || request.Items.Count == 0)
            {
                return Results.BadRequest(new { error = "Items are required." });
            }

            var items = request.Items
                .Select(item => new OrderItemDto(item.Sku, item.Quantity, item.UnitPrice))
                .ToArray();

            var result = await handler.HandleAsync(new StartCheckoutCommand(items), cancellationToken);

            return Results.Accepted(
                $"/api/checkout/{result.OrderId}",
                new StartCheckoutResponse(result.OrderId, result.Status.ToString(), result.Total));
        });

        return endpoints;
    }
}
