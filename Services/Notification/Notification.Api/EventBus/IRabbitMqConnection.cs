using RabbitMQ.Client;

namespace Notification.Api.EventBus;

public interface IRabbitMqConnection : IDisposable
{
    Task<IConnection> CreateConnection();
}