namespace SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

using Microsoft.AspNetCore.SignalR;
using SampleSystemDesign.RealTime.Application.Interfaces;
using SampleSystemDesign.RealTime.Domain.ValueObjects;

public sealed class MarketDataPublisher : IMarketDataPublisher
{
    private readonly IHubContext<MarketDataHub> hubContext;

    public MarketDataPublisher(IHubContext<MarketDataHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task PublishAsync(MarketData marketData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(marketData);

        return hubContext.Clients.All.SendAsync("marketData", marketData, cancellationToken);
    }
}
