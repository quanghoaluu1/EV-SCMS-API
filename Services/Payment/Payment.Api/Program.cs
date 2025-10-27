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
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<BookingGrpc.BookingGrpcService.BookingGrpcServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration.GetConnectionString("BookingHost"));
});
builder.Services.AddGrpcClient<CustomerGrpc.CustomerService.CustomerServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration.GetConnectionString("CustomerHost"));
});
builder.Services.AddScoped<IBookingGrpcGateway, BookingGrpcGateway>();
builder.Services.AddScoped<ICustomerGrpcGateway, CustomerGrpcGateway>();
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();