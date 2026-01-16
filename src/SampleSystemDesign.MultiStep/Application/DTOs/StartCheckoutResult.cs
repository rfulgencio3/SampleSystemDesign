namespace SampleSystemDesign.MultiStep.Application.DTOs;

using SampleSystemDesign.MultiStep.Domain.Entities;

public sealed record StartCheckoutResult(Guid OrderId, OrderStatus Status, decimal Total);
