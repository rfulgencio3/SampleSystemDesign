using SampleSystemDesign.ScalingReads.Domain.Entities;
using SampleSystemDesign.ScalingReads.Domain.Interfaces;
using SampleSystemDesign.ScalingReads.Infrastructure.Configuration;
using SampleSystemDesign.ScalingReads.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScalingReads(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await SeedDataAsync(app.Services);

app.MapShortUrlEndpoints();

app.Run();

static async Task SeedDataAsync(IServiceProvider services)
{
    var repository = services.GetRequiredService<IShortUrlRepository>();
    var now = DateTimeOffset.UtcNow;

    await repository.SaveAsync(new ShortUrl(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "https://events.example.com/schedule", "sched", now.AddDays(7)));
    await repository.SaveAsync(new ShortUrl(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "https://events.example.com/registration", "reg", now.AddDays(30)));
    await repository.SaveAsync(new ShortUrl(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), "https://events.example.com/venue", "venue", null));
}

