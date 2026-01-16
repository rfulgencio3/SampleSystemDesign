namespace SampleSystemDesign.MultiStep.Application.DTOs;

public sealed record CheckoutMessage(Guid OrderId, string Step, string Status, string? Detail);
