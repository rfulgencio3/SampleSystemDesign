namespace SampleSystemDesign.MultiStep.Domain.Entities;

public sealed class OrderItem
{
    public OrderItem(string sku, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("Sku is required.", nameof(sku));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice));

        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public string Sku { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal LineTotal => UnitPrice * Quantity;
}
