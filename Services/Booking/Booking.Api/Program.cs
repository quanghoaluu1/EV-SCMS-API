using Booking.Application.IServices;
using Booking.Application.Mapping;
using Booking.Application.Services;
using Booking.Infrastructure;
using Booking.Infrastructure.IRepository;
using Booking.Infrastructure.IServices;
using Booking.Infrastructure.Repository;
using Booking.Infrastructure.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<BookingServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookingService, BookingServices>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Configure JSON serialization to use string enums
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

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

builder.Services.AddHttpClient<IStaffGrpcGateway, StaffGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["StaffHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var config = TypeAdapterConfig.GlobalSettings;
MappingConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Booking API",
        Version = "v1",
        Description = "Booking Management API"
    });
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