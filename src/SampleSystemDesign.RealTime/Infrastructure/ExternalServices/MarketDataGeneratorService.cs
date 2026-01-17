namespace SampleSystemDesign.RealTime.Infrastructure.ExternalServices;

using Microsoft.Extensions.Hosting;
using SampleSystemDesign.RealTime.Application.Interfaces;
using SampleSystemDesign.RealTime.Domain.ValueObjects;

public sealed class MarketDataGeneratorService : BackgroundService
{
    private static readonly string[] Symbols = { "EVT", "TIX", "GLOB", "SPRK", "VENU" };
    private readonly IMarketDataPublisher publisher;
    private readonly TimeSpan interval;
    private readonly Random random = new();

    public MarketDataGeneratorService(IMarketDataPublisher publisher, TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(interval));

        this.publisher = publisher;
        this.interval = interval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var symbol = Symbols[random.Next(Symbols.Length)];
            var price = Math.Round((decimal)(random.NextDouble() * 90.0 + 10.0), 2);
            var change = Math.Round((decimal)(random.NextDouble() * 2.0 - 1.0), 2);

            var marketData = MarketData.Create(symbol, price, change);

            await publisher.PublishAsync(marketData, stoppingToken);
            await Task.Delay(interval, stoppingToken);
        }
    }
}
