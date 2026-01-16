namespace SampleSystemDesign.MultiStep.Application.UseCases;

using SampleSystemDesign.MultiStep.Application.DTOs;

public sealed record StartCheckoutCommand(IReadOnlyCollection<OrderItemDto> Items);
