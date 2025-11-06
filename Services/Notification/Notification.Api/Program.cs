using Mapster;
using Microsoft.EntityFrameworkCore;
using Notification.Api.Consumers;
using Notification.Api.EventBus;
using Notification.Application.Hubs;
using Notification.Application.IServices;
using Notification.Application.Mapping;
using Notification.Application.Services;
using Notification.Infrastructure;
using Notification.Infrastructure.IRepository;
using Notification.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddDbContext<NotificationDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddHostedService<EventConsumer>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b =>
        b.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials());
});

var config = TypeAdapterConfig.GlobalSettings;
MappingConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok("Healthy ✅"));
app.UseCors("AllowAll");
app.MapHub<NotificationHub>("/notifyHub");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();