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
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MaintenanceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

var config = TypeAdapterConfig.GlobalSettings;
MappingConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);

// Configure JSON serialization to use string enums
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for external service communication
builder.Services.AddHttpClient<ICustomerGrpcGateway, CustomerGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CustomerHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IVehicleGrpcGateway, VehicleGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["VehicleHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IBookingGrpcGateway, BookingGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BookingHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IStaffGrpcGateway, StaffGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["StaffHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();