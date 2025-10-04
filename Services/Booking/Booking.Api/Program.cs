using Booking.Application.IServices;
using Booking.Application.Mapping;
using Booking.Application.Services;
using Booking.Infrastructure;
using Booking.Infrastructure.IRepository;
using Booking.Infrastructure.Repository;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<BookingServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookingService, BookingServices>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddControllers();

var config = TypeAdapterConfig.GlobalSettings;
MappingConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();