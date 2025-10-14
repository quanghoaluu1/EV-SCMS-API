using Auth.Api.GrpcServices;
using Auth.Application.IServices;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Infrastructure;
using Auth.Infrastructure.GrpcClients;
using Auth.Infrastructure.Interfaces;
using Auth.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaffGrpc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpcClient<StaffGrpc.Staff.StaffClient>(o =>
{
    o.Address = new Uri("https://localhost:7242");
});
builder.Services.AddGrpcClient<CustomerGrpc.CustomerService.CustomerServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:7040");
});
builder.Services.AddScoped<IStaffGrpcGateway,StaffGrpcGateway>();
builder.Services.AddScoped<ICustomerGrpcGateway, CustomerGrpcGateway>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<AuthGrpcService>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();