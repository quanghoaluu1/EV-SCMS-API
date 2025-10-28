using Booking.Application.IServices;
using Booking.Application.Mapping;
using Booking.Application.Services;
using Booking.Infrastructure;
using Booking.Infrastructure.IRepository;
using Booking.Infrastructure.IServices;
using Booking.Infrastructure.Repository;
using Booking.Infrastructure.Services;
using Booking.Api.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using CustomerGrpc;
using VehicleGrpc;
using StaffGrpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<BookingServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookingService, BookingServices>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddControllers();

// Add gRPC services
builder.Services.AddGrpc();

// Add gRPC clients for external service validation
builder.Services.AddGrpcClient<CustomerService.CustomerServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["CustomerHost"]); // Customer service URL
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
    options.Address = new Uri(builder.Configuration["VehicleHost"]); // Vehicle service URL
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
    options.Address = new Uri(builder.Configuration["StaffHost"]); // Staff service URL
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return handler;
});

// Register gRPC gateways
builder.Services.AddScoped<ICustomerGrpcGateway, CustomerGrpcGateway>();
builder.Services.AddScoped<IVehicleGrpcGateway, VehicleGrpcGateway>();
builder.Services.AddScoped<IStaffGrpcGateway, StaffGrpcGateway>();

var config = TypeAdapterConfig.GlobalSettings;
MappingConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/health", () => Results.Ok("Healthy ✅"));
// Map gRPC services
app.MapGrpcService<BookingGrpcService>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();