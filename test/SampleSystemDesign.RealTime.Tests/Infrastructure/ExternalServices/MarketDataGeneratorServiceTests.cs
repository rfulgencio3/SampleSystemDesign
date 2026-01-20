using SampleSystemDesign.RealTime.Application.Interfaces;
using SampleSystemDesign.RealTime.Domain.ValueObjects;
using SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

namespace SampleSystemDesign.RealTime.Tests.Infrastructure.ExternalServices;

public class MarketDataGeneratorServiceTests
{
    [Fact]
    public async Task ExecuteAsync_PublishesMarketData()
    {
        var publisher = new FakeMarketDataPublisher();
        var generator = new MarketDataGeneratorService(publisher, TimeSpan.FromMilliseconds(10));
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(60));

        await generator.StartAsync(cts.Token);
        await Task.Delay(40, cts.Token);
        await generator.StopAsync(CancellationToken.None);

        Assert.NotEmpty(publisher.Published);
    }

    private sealed class FakeMarketDataPublisher : IMarketDataPublisher
    {
        public List<MarketData> Published { get; } = new();

        public Task PublishAsync(MarketData marketData, CancellationToken cancellationToken = default)
        {
            Published.Add(marketData);
            return Task.CompletedTask;
        }
    }
}
