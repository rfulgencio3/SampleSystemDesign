namespace SampleSystemDesign.MultiStep.Presentation;

public sealed record CheckoutItemRequest(string Sku, int Quantity, decimal UnitPrice);

public sealed record StartCheckoutRequest(IReadOnlyCollection<CheckoutItemRequest> Items);
