namespace SampleSystemDesign.RealTime.Presentation;

using Microsoft.AspNetCore.Builder;
using SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

public static class HubEndpoints
{
    public static IEndpointRouteBuilder MapRealTimeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<MarketDataHub>("/hub/market-data");
        return endpoints;
    }
}
