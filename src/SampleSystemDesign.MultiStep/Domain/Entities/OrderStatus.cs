namespace SampleSystemDesign.MultiStep.Domain.Entities;

public enum OrderStatus
{
    Pending,
    PaymentAuthorized,
    InventoryReserved,
    Completed,
    Failed
}
