using Auth.Domain.Enums;

namespace Auth.Application.DTOs.Otp;

public class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; } = OtpPurpose.Registration;
}