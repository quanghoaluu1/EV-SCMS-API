namespace Messaging;

public interface IMessageBus
{
    Task Publish(string queue, object message);
}