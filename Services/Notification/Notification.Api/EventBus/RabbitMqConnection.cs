using RabbitMQ.Client;

namespace Notification.Api.EventBus;

public class RabbitMqConnection : IRabbitMqConnection
{
    private IConnection _connection;

    public RabbitMqConnection()
    {
        
    }

    public async Task<IConnection> CreateConnection()
    {
        if (_connection != null && _connection.IsOpen)
        {
            return _connection;
        }

        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "12345"
        };
        _connection = await factory.CreateConnectionAsync();
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}