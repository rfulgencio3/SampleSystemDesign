using SampleSystemDesign.LongRunning.Infrastructure.Configuration;
using SampleSystemDesign.LongRunning.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLongRunning(builder.Configuration);

var app = builder.Build();

app.MapImageJobEndpoints();

app.Run();
