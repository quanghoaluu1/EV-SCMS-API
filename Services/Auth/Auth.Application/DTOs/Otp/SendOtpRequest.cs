using Auth.Domain.Enums;

namespace Auth.Application.DTOs.Otp;

public class SendOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; } = OtpPurpose.Registration;
}