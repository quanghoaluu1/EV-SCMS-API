using Auth.Application.DTOs.Otp;
using Auth.Domain.Enums;

namespace Auth.Application.Interfaces;

public interface IOtpService
{
    Task<OtpResponse> SendOtpAsync(string email, OtpPurpose purpose);
    Task<OtpResponse> VerifyOtpAsync(string email, string otp, OtpPurpose purpose);
    Task<bool> InvalidateOtpAsync(string email, OtpPurpose purpose);
    Task<int?> GetRemainingCooldownAsync(string email, OtpPurpose purpose);
}