using Customer.Api.Services;
using Customer.Application.IServices;
using Customer.Application.Services;
using Customer.Infrastructure;
using Customer.Infrastructure.IRepository;
using Customer.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<AuthGrpc.Auth.AuthClient>(o =>
{
    o.Address = new Uri("https://localhost:7212");
});
builder.Services.AddScoped<IAuthGrpcGateway, AuthGrpcGateway>();
// Add gRPC services
builder.Services.AddGrpc();

builder.Services.AddDbContext<CustomerServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map gRPC services
app.MapGrpcService<CustomerGrpcService>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();