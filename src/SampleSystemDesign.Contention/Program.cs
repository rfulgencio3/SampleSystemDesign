using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Domain.Interfaces;
using SampleSystemDesign.Contention.Infrastructure.Configuration;
using SampleSystemDesign.Contention.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddContention(builder.Configuration);

var app = builder.Build();

await SeedDataAsync(app.Services);

app.MapTicketReservationEndpoints();

app.Run();

static async Task SeedDataAsync(IServiceProvider services)
{
    var repository = services.GetRequiredService<ITicketInventoryRepository>();

    var eventId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    await repository.SaveAsync(new TicketInventory(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), eventId, 100, 100, 0));
    await repository.SaveAsync(new TicketInventory(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Guid.Parse("22222222-2222-2222-2222-222222222222"), 50, 0, 0));
}
