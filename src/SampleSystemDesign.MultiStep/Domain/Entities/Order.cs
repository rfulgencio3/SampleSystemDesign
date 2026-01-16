namespace SampleSystemDesign.MultiStep.Domain.Entities;

public sealed class Order
{
    public Order(Guid id, OrderStatus status, IReadOnlyCollection<OrderItem> items, decimal total)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (items is null) throw new ArgumentNullException(nameof(items));

        var itemList = items.ToArray();
        if (itemList.Length == 0)
        {
            throw new ArgumentException("Order must contain at least one item.", nameof(items));
        }

        if (total < 0) throw new ArgumentOutOfRangeException(nameof(total));

        var computedTotal = itemList.Sum(item => item.LineTotal);
        if (computedTotal != total)
        {
            throw new ArgumentException("Order total does not match item totals.", nameof(total));
        }

        Id = id;
        Status = status;
        Items = itemList;
        Total = total;
    }

    public Guid Id { get; }
    public OrderStatus Status { get; }
    public IReadOnlyCollection<OrderItem> Items { get; }
    public decimal Total { get; }

    public Order WithStatus(OrderStatus status) => new(Id, status, Items, Total);
}
