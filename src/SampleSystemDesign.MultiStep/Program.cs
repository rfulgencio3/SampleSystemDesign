using SampleSystemDesign.MultiStep.Infrastructure.Configuration;
using SampleSystemDesign.MultiStep.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMultiStep(builder.Configuration);

var app = builder.Build();

app.MapCheckoutEndpoints();

app.Run();
