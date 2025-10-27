using RabbitMQ.Client;

namespace Notification.Api.EventBus;

public class RabbitMqConnection : IRabbitMqConnection
{
    private IConnection _connection;
    private readonly IConfiguration _configuration;

    public RabbitMqConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IConnection> CreateConnection()
    {
        if (_connection != null && _connection.IsOpen)
        {
            return _connection;
        }

        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        _connection = await factory.CreateConnectionAsync();
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}