using SampleSystemDesign.ScalingReads.Domain.Entities;
using SampleSystemDesign.ScalingReads.Infrastructure.Configuration;
using SampleSystemDesign.ScalingReads.Infrastructure.Persistence;
using SampleSystemDesign.ScalingReads.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScalingReads();

var app = builder.Build();

SeedData(app.Services);

app.MapShortUrlEndpoints();

app.Run();

static void SeedData(IServiceProvider services)
{
    var repository = services.GetRequiredService<InMemoryShortUrlRepository>();
    var now = DateTimeOffset.UtcNow;

    repository.Seed(
        new[]
        {
            new ShortUrl(Guid.NewGuid(), "https://events.example.com/schedule", "sched", now.AddDays(7)),
            new ShortUrl(Guid.NewGuid(), "https://events.example.com/registration", "reg", now.AddDays(30)),
            new ShortUrl(Guid.NewGuid(), "https://events.example.com/venue", "venue", null)
        });
}
