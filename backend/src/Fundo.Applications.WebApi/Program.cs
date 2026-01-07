using Fundo.Applications.Application;
using Fundo.Applications.Infrastructure;
using Fundo.Applications.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel endpoints explicitly (listen on any IP for container networking)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
    options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
});

// Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); ; ;

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi("/openapi.json");
    app.MapScalarApiReference(options =>
    {
        options.OpenApiRoutePattern = "/openapi.json";
    });
}

app.MigrateDatabase();

app.UseCors("corsOrigins");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled WebApi exception: {ex.Message}");
}
