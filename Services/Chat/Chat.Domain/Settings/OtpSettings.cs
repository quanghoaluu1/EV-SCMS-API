namespace Chat.Domain.Settings;

public class OtpSettings
{
    public int OtpLength { get; set; } = 6;
    public int ExpiryMinutes { get; set; } = 5;
    public int MaxAttempts { get; set; } = 3;
    public int ResendCooldownSeconds { get; set; } = 60;
}