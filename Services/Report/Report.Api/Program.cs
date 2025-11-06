using Report.Application.IServices;
using Report.Application.Services;
using Report.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for external service communication
builder.Services.AddHttpClient<IBookingGrpcGateway, BookingGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BookingHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IMaintenanceGrpcGateway, MaintenanceGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MaintenanceHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register application services
builder.Services.AddScoped<IReportService, ReportService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();