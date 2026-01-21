namespace SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

using Microsoft.AspNetCore.SignalR;
using SampleSystemDesign.RealTime.Application.Interfaces;
using SampleSystemDesign.RealTime.Domain.ValueObjects;

public sealed class MarketDataPublisher(IHubContext<MarketDataHub> hubContext) : IMarketDataPublisher
{
    private readonly IHubContext<MarketDataHub> hubContext = hubContext;

    public Task PublishAsync(MarketData marketData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(marketData);

        return hubContext.Clients.All.SendAsync("marketData", marketData, cancellationToken);
    }
}
