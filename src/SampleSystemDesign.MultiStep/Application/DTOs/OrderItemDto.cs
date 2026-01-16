namespace SampleSystemDesign.MultiStep.Application.DTOs;

public sealed record OrderItemDto(string Sku, int Quantity, decimal UnitPrice);
