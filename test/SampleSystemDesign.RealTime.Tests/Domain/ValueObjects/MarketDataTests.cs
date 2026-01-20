using SampleSystemDesign.RealTime.Domain.ValueObjects;

namespace SampleSystemDesign.RealTime.Tests.Domain.ValueObjects;

public class MarketDataTests
{
    [Fact]
    public void Create_RequiresValidSymbol()
    {
        Assert.Throws<ArgumentException>(() => MarketData.Create(" ", 10m, 1m));
    }
}
