using Chat.Domain.Enums;

namespace Chat.Domain.DTOs;

public class SendOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; } = OtpPurpose.Registration;
}