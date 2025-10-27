using BookingGrpc;
using MaintenanceGrpc;
using Report.Application.IServices;
using Report.Application.Services;
using Report.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.ClientFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure GRPC clients
var bookingServiceUrl = builder.Configuration.GetConnectionString("BookingService") ?? "https://localhost:7001";
var maintenanceServiceUrl = builder.Configuration.GetConnectionString("MaintenanceService") ?? "https://localhost:7002";

builder.Services.AddGrpcClient<BookingGrpcService.BookingGrpcServiceClient>(options =>
{
    options.Address = new Uri(bookingServiceUrl);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    // Allow untrusted certificates for development
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

builder.Services.AddGrpcClient<MaintenanceGrpcService.MaintenanceGrpcServiceClient>(options =>
{
    options.Address = new Uri(maintenanceServiceUrl);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    // Allow untrusted certificates for development
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

// Register application services
builder.Services.AddScoped<IReportService, ReportService>();

// Register infrastructure services (GRPC gateways)
builder.Services.AddScoped<IBookingGrpcGateway, BookingGrpcGateway>();
builder.Services.AddScoped<IMaintenanceGrpcGateway, MaintenanceGrpcGateway>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();