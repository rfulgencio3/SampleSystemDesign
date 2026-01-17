namespace SampleSystemDesign.RealTime.Domain.ValueObjects;

public sealed record MarketData(string Symbol, decimal Price, decimal Change)
{
    public static MarketData Create(string symbol, decimal price, decimal change)
    {
        if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException("Symbol is required.", nameof(symbol));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        return new MarketData(symbol, price, change);
    }
}
