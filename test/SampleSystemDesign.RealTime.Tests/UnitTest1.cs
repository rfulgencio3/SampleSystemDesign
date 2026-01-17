using SampleSystemDesign.RealTime.Application.Interfaces;
using SampleSystemDesign.RealTime.Domain.ValueObjects;
using SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

namespace SampleSystemDesign.RealTime.Tests;

public class RealTimeTests
{
    [Fact]
    public async Task Generator_PublishesMarketData()
    {
        var publisher = new FakeMarketDataPublisher();
        var generator = new MarketDataGeneratorService(publisher, TimeSpan.FromMilliseconds(10));
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(60));

        await generator.StartAsync(cts.Token);
        await Task.Delay(40, cts.Token);
        await generator.StopAsync(CancellationToken.None);

        Assert.NotEmpty(publisher.Published);
    }

    [Fact]
    public void MarketData_RequiresValidSymbol()
    {
        Assert.Throws<ArgumentException>(() => MarketData.Create(" ", 10m, 1m));
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
