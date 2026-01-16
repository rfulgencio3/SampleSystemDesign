using SampleSystemDesign.MultiStep.Infrastructure.Configuration;
using SampleSystemDesign.MultiStep.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMultiStep();

var app = builder.Build();

app.MapCheckoutEndpoints();

app.Run();
