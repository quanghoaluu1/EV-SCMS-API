using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Vehicle.Application.IServices;
using Vehicle.Application.Mapping;
using Vehicle.Application.Services;
using Vehicle.Infrashtructure;
using Vehicle.Infrashtructure.IRepository;
using Vehicle.Infrashtructure.Repository;
using VehicleGrpc.API.Services;
//using VehicleGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add gRPC client for Auth service
builder.Services.AddGrpcClient<AuthGrpc.Auth.AuthClient>(o =>
{
    o.Address = new Uri("https://localhost:7212");
});
builder.Services.AddScoped<IAuthGrpcGateway, AuthGrpcGateway>();

// Add gRPC services
builder.Services.AddGrpc();

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

// Map gRPC services
app.MapGrpcService<VehicleGrpcService>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();