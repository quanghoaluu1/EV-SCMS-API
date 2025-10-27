using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging;

public class RabbitMqConsumer
{
    private readonly ConnectionFactory _connectionFactory;
    public RabbitMqConsumer(string uri)
    {
        _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(uri)
        };
    }

    public async Task ConsumeAsync<T>(string queue, Func<T, Task> handler)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(queue, false, false, false);
        
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
            if (message != null)
            {
                await handler(message);
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        };
        await channel.BasicConsumeAsync(queue, autoAck: false, consumer);
    }
}