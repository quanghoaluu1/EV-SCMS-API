using Chat.Domain.DTOs;
using Chat.Domain.Enums;

namespace Chat.Application.IServices;

public interface IOtpService
{
    Task<OtpResponse> SendOtpAsync(string email, OtpPurpose purpose);
    Task<OtpResponse> VerifyOtpAsync(string email, string otp, OtpPurpose purpose);
    Task<bool> InvalidateOtpAsync(string email, OtpPurpose purpose);
    Task<int?> GetRemainingCooldownAsync(string email, OtpPurpose purpose);
}