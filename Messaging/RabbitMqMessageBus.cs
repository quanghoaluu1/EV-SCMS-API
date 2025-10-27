using System.Net.Mime;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Messaging;

public class RabbitMqMessageBus : IMessageBus
{
    private readonly IConnectionFactory _connectionFactory;
    public RabbitMqMessageBus(string uri)
    {
        _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(uri)
        };
    }
    
    public async Task Publish(string queue, object message)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var props = new BasicProperties(
        );
        
        await channel.BasicPublishAsync<BasicProperties>(
            exchange: "",
            routingKey: queue,
            mandatory: false,
            basicProperties: props,
            body: body);
        
        Console.WriteLine($"[x] Sent message to queue {queue}");
    }
}