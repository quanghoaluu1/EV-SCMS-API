using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Notification.Application.Hubs;
using RabbitMQ.Client.Events;

namespace Notification.Api.EventBus;

public class EventConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _connection;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<EventConsumer> _logger;
    
    public EventConsumer(IRabbitMqConnection connection, IHubContext<NotificationHub> hubContext, ILogger<EventConsumer> logger)
    {
        _connection = connection;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async  Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = await _connection.CreateConnection();
        var channel = await connection.CreateChannelAsync();

        var queue = "booking_created";
        await channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var evt = JsonSerializer.Deserialize<IntegrationEvent>(json);
                if (evt != null && evt.EventName == "BookingCreated")
                {
                    _logger.LogInformation("Received event: {EventName} with data: {EventData}", evt.EventName,
                        evt.Data);
                    
                    // Parse the booking data
                    var bookingData = JsonSerializer.Deserialize<JsonElement>(evt.Data);
                    var customerId = bookingData.GetProperty("CustomerId").GetGuid();
                    var bookingId = bookingData.GetProperty("BookingId").GetGuid();
                    var scheduledDate = bookingData.GetProperty("ScheduledDate").GetDateTime();
                    
                    // Send notification to all clients
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                    {
                        Title = "Đặt lịch thành công",
                        Message = $"Booking #{bookingId} đã được tạo cho ngày {scheduledDate:dd/MM/yyyy HH:mm}",
                        CustomerId = customerId,
                        BookingId = bookingId,
                        Timestamp = DateTime.UtcNow
                    });
                    
                    _logger.LogInformation("✅ Notification sent to all clients for booking {BookingId}", bookingId);
                }
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        };

       await channel.BasicConsumeAsync(queue: queue, autoAck: false, consumerTag: "", noLocal:false, exclusive:false,arguments:null, consumer: consumer);
       _logger.LogInformation("✅ NotificationService đang lắng nghe queue: {Queue}", queue);
    }
}