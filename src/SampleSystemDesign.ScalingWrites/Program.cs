using SampleSystemDesign.ScalingWrites.Infrastructure.Configuration;
using SampleSystemDesign.ScalingWrites.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScalingWrites(builder.Configuration);

var app = builder.Build();

app.MapTelemetryEndpoints();

app.Run();
