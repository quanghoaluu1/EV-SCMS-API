using Chat.Domain.Enums;

namespace Chat.Domain.DTOs;

public class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; } = OtpPurpose.Registration;
}