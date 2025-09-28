using Microsoft.EntityFrameworkCore;
using Vehicle.Api.Services;
using Vehicle.Application.IServices;
using Vehicle.Application.Mapping;
using Vehicle.Application.Services;
using Vehicle.Infrashtructure;
using Vehicle.Infrashtructure.IRepository;
using Vehicle.Infrashtructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add gRPC
builder.Services.AddGrpc();

builder.Services.AddDbContext<VehicleServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, Vehicle.Application.Services.VehicleService>();

// Configure Mapster
VehicleMapping.Configure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configure gRPC
app.MapGrpcService<VehicleGrpcService>();

app.Run();