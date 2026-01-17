namespace SampleSystemDesign.RealTime.Application.Interfaces;

using SampleSystemDesign.RealTime.Domain.ValueObjects;

public interface IMarketDataPublisher
{
    Task PublishAsync(MarketData marketData, CancellationToken cancellationToken = default);
}
