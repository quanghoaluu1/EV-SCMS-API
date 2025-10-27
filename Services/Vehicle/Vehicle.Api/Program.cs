using Microsoft.EntityFrameworkCore;
using Vehicle.Application.IServices;
using Vehicle.Application.Mapping;
using Vehicle.Application.Services;
using Vehicle.Infrashtructure;
using Vehicle.Infrashtructure.IRepository;
using Vehicle.Infrashtructure.Repository;
using Vehicle.Infrastructure.IService;
using Vehicle.Infrastructure.Service;
using VehicleGrpc.API.Services;
using CustomerGrpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add gRPC services
builder.Services.AddGrpc();

// Add gRPC client for Customer service with improved configuration
builder.Services.AddGrpcClient<CustomerService.CustomerServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7040");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    // For development - ignore SSL certificate errors
    handler.ServerCertificateCustomValidationCallback = 
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return handler;
})
.ConfigureChannel(options =>
{
    // Configure gRPC channel options
    options.UnsafeUseInsecureChannelCallCredentials = true;
    options.MaxReceiveMessageSize = 4 * 1024 * 1024; // 4 MB
    options.MaxSendMessageSize = 4 * 1024 * 1024; // 4 MB
});

builder.Services.AddScoped<ICustomerGrpcGateway, CustomerGrpcGateway>();

builder.Services.AddDbContext<VehicleServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Configure Mapster
VehicleMapping.Configure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

// Map gRPC services
app.MapGrpcService<VehicleGrpcService>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();