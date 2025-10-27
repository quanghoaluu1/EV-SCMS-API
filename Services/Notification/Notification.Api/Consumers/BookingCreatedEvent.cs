using Messaging;

namespace Notification.Api.Consumers;

public class BookingCreatedEvent
{
    public string EventType { get; set; } = default!;
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = default!;
}

public class BookingCreatedConsumer
{
    private readonly RabbitMqConsumer _consumer;
    private readonly IConfiguration _configuration;

    public BookingCreatedConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
        _consumer = new RabbitMqConsumer(_configuration["RabbitMQ:ConnectionString"] ?? "localhost");
    }
    public async Task StartAsync()
    {
        await _consumer.ConsumeAsync<BookingCreatedEvent>("booking_created", async message =>
        {
            Console.WriteLine($"📩 [BookingCreatedConsumer] Gửi thông báo cho user {message.CustomerId}");
            await Task.CompletedTask;
        });
    }
}