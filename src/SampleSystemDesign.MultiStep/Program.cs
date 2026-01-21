using SampleSystemDesign.MultiStep.Infrastructure.Configuration;
using SampleSystemDesign.MultiStep.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMultiStep(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCheckoutEndpoints();

app.Run();

