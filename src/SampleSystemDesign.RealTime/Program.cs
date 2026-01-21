using SampleSystemDesign.RealTime.Infrastructure.Configuration;
using SampleSystemDesign.RealTime.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRealTime(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapRealTimeEndpoints();

app.Run();

