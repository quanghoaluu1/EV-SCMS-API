using Auth.Application.Interfaces;
using Auth.Application.Services;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Auth.Infrastructure;
using Auth.Infrastructure.GrpcClients;
using Auth.Infrastructure.Interfaces;
using Auth.Infrastructure.Repository;
using Chat.Domain.Settings;
using Common.Redis;
using Common.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add HttpClient for external service communication
builder.Services.AddHttpClient<IStaffGrpcGateway, StaffGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["StaffHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<ICustomerGrpcGateway, CustomerGrpcGateway>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CustomerHost"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGrid"));
builder.Services.Configure<OtpSettings>(
    builder.Configuration.GetSection("OtpSettings"));
builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection("RedisSettings"));
string connectionString = builder.Configuration.GetSection("RedisSettings").GetValue<string>("ConnectionString")
                          ?? throw new ArgumentNullException("Redis connection string is not configured.");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IRedisService, RedisService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Auth API",
        Version = "v1",
        Description = "Authentication and Authorization API"
    });
});
builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok("Healthy ✅"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();