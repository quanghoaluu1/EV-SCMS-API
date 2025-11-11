using Customer.Application.IServices;
using Customer.Application.Services;
using Customer.Infrastructure;
using Customer.Infrastructure.IRepository;
using Customer.Infrastructure.Repository;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Customer API",
        Version = "v1",
        Description = "Customer Management API"
    });
});

// Add HttpClient for external service communication
builder.Services.AddHttpClient<IAuthGrpcGateway, AuthGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AuthHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddDbContext<CustomerServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

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