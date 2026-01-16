using SampleSystemDesign.LargeFiles.Infrastructure.Configuration;
using SampleSystemDesign.LargeFiles.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLargeFiles();

var app = builder.Build();

app.MapAssetEndpoints();

app.Run();
