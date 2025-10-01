// ApiGateway/Program.cs
using ApiGateway.Services;
using Microsoft.Extensions.DependencyInjection; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure gRPC Clients
builder.Services.AddGrpcClient<InventoryService.Grpc.InventoryService.InventoryServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration.GetValue<string>("GrpcServices:InventoryService")
                            ?? "https://localhost:5001");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // For development - ignore SSL certificate errors
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

builder.Services.AddGrpcClient<VehicleService.Grpc.VehicleService.VehicleServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration.GetValue<string>("GrpcServices:VehicleService")
                            ?? "https://localhost:5002");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

builder.Services.AddGrpcClient<AuthService.Grpc.AuthService.AuthServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration.GetValue<string>("GrpcServices:AuthService")
                            ?? "https://localhost:5003");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

// Register wrapper services
builder.Services.AddScoped<IInventoryGrpcClient, InventoryGrpcClient>();
builder.Services.AddScoped<IVehicleGrpcClient, VehicleGrpcClient>();
builder.Services.AddScoped<IAuthGrpcClient, AuthGrpcClient>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();