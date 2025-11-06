using Microsoft.EntityFrameworkCore;
using Payment.Application.Interfaces;
using Payment.Application.Services;
using Payment.Infrastructure;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Repositories;
using Payment.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Payment API",
        Version = "v1",
        Description = "Payment Processing API"
    });
});

// Add HttpClient for external service communication
builder.Services.AddHttpClient<IBookingGrpcGateway, BookingGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BookingHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<ICustomerGrpcGateway, CustomerGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CustomerHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();