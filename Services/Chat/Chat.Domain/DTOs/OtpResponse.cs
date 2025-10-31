namespace Chat.Domain.DTOs;

public class OtpResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ExpiresInSeconds { get; set; }
    public int? RemainingAttempts { get; set; }
    public int? CooldownSeconds { get; set; }
}