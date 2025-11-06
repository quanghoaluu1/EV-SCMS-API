using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Vehicle.Application.IServices;
using Vehicle.Application.Mapping;
using Vehicle.Application.Services;
using Vehicle.Infrashtructure;
using Vehicle.Infrashtructure.IRepository;
using Vehicle.Infrashtructure.Repository;
using Vehicle.Infrastructure.IService;
using Vehicle.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Vehicle API",
        Version = "v1",
        Description = "Vehicle Management API"
    });
});

// Add HttpClient for external service communication
builder.Services.AddHttpClient<ICustomerGrpcGateway, CustomerGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CustomerHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddDbContext<VehicleServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Configure Mapster
VehicleMapping.Configure();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();