namespace Notification.Api.EventBus;

public class IntegrationEvent
{
    public string EventName { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}