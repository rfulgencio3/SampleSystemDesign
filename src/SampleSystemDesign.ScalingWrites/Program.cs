using SampleSystemDesign.ScalingWrites.Infrastructure.Configuration;
using SampleSystemDesign.ScalingWrites.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScalingWrites(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTelemetryEndpoints();

app.Run();

