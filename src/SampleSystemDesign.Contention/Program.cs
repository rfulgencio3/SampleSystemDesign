using SampleSystemDesign.Contention.Domain.Entities;
using SampleSystemDesign.Contention.Infrastructure.Configuration;
using SampleSystemDesign.Contention.Infrastructure.Persistence;
using SampleSystemDesign.Contention.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddContention();

var app = builder.Build();

SeedData(app.Services);

app.MapTicketReservationEndpoints();

app.Run();

static void SeedData(IServiceProvider services)
{
    var repository = services.GetRequiredService<InMemoryTicketInventoryRepository>();

    var eventId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    repository.Seed(new[]
    {
        new TicketInventory(Guid.NewGuid(), eventId, 100, 100, 0),
        new TicketInventory(Guid.NewGuid(), Guid.Parse("22222222-2222-2222-2222-222222222222"), 50, 0, 0)
    });
}
