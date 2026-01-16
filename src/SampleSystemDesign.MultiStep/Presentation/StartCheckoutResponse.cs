namespace SampleSystemDesign.MultiStep.Presentation;

public sealed record StartCheckoutResponse(Guid OrderId, string Status, decimal Total);
