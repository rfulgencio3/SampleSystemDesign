using SampleSystemDesign.RealTime.Infrastructure.Configuration;
using SampleSystemDesign.RealTime.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRealTime(builder.Configuration);

var app = builder.Build();

app.MapRealTimeEndpoints();

app.Run();
