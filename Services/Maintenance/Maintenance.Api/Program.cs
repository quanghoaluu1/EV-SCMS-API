using BookingGrpc;
using Maintenance.Application.IServices;
using Maintenance.Application.Mapping;
using Maintenance.Application.Services;
using Maintenance.Infrastructure;
using Maintenance.Infrastructure.IRepository;
using Maintenance.Infrastructure.IServices;
using Maintenance.Infrastructure.Repository;
using Maintenance.Infrastructure.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using CustomerGrpc;
using VehicleGrpc;
using BookingGrpc;
using StaffGrpc;
using Maintenance.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MaintenanceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

var config = TypeAdapterConfig.GlobalSettings;
MappingConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ADD gRPC services
builder.Services.AddGrpc();

// ADD gRPC clients for external service validation
builder.Services.AddGrpcClient<CustomerService.CustomerServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7040"); // Customer service URL
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return handler;
});

builder.Services.AddGrpcClient<VehicleGrpcService.VehicleGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7039"); // Vehicle service URL
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return handler;
});

builder.Services.AddGrpcClient<BookingGrpcService.BookingGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7041"); // Booking service URL  
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return handler;
});

builder.Services.AddGrpcClient<Staff.StaffClient>(options =>
{
    options.Address = new Uri("https://localhost:7242"); // Staff service URL
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return handler;
});

// ADD Register gRPC gateways
builder.Services.AddScoped<ICustomerGrpcGateway, CustomerGrpcGateway>();
builder.Services.AddScoped<IVehicleGrpcGateway, VehicleGrpcGateway>();
builder.Services.AddScoped<IBookingGrpcGateway, BookingGrpcGateway>();
builder.Services.AddScoped<IStaffGrpcGateway, StaffGrpcGateway>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<MaintenanceGrpcService>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();