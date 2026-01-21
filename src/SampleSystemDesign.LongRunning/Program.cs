using SampleSystemDesign.LongRunning.Infrastructure.Configuration;
using SampleSystemDesign.LongRunning.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLongRunning(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapImageJobEndpoints();

app.Run();

