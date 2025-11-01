using System.Security.Cryptography;
using System.Text.Json;
using Auth.Application.DTOs.Otp;
using Auth.Application.Interfaces;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Chat.Domain.Settings;
using Common.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.Application.Services;

public class OtpService : IOtpService
{
    private readonly IRedisService _redisService;
    private readonly IEmailService _emailService;
    private readonly OtpSettings _otpSettings;
    private readonly ILogger<OtpService> _logger;
    
    private const string OtpPrefix = "otp:";
    private const string CooldownPrefix = "otp_cooldown:";
    private const string AttemptsPrefix = "otp_attempts:";

    public OtpService(IRedisService redisService, IEmailService emailService, IOptions<OtpSettings> otpSettings,
        ILogger<OtpService> logger)
    {
        _redisService = redisService;
        _emailService = emailService;
        _otpSettings = otpSettings.Value;
        _logger = logger;
    }
    
    public async Task<OtpResponse> SendOtpAsync(string email, OtpPurpose purpose)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                return new OtpResponse()
                {
                    Success = false,
                    Message = "Email không hợp lệ"
                };
            }
            var cooldownKey = GetCooldownKey(email, purpose);
            var cooldownRemaining = await GetRemainingCooldownAsync(email, purpose);

            if (cooldownRemaining.HasValue && cooldownRemaining.Value > 0)
            {
                return new OtpResponse()
                {
                    Success = false,
                    Message = $"Vui lòng đợi {cooldownRemaining.Value} giây trước khi gửi lại",
                    RemainingAttempts = cooldownRemaining.Value
                };
            }
            var otp = GenerateOtp(_otpSettings.OtpLength);
            var otpData = new OtpData()
            {
                Email = email,
                Otp = otp,
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes),
                Attempts = 0,
                IsVerified = false
            };
            
            var otpKey = GetOtpKey(email, purpose);
            var otpJson = JsonSerializer.Serialize(otpData);
            var expiry = TimeSpan.FromMinutes(_otpSettings.ExpiryMinutes);
            
            await _redisService.SetStringAsync(otpKey, otpJson, expiry);
            await _redisService.SetStringAsync(cooldownKey, DateTime.UtcNow.ToString("o") , TimeSpan.FromSeconds(_otpSettings.ResendCooldownSeconds));
            
            var emailSent = await _emailService.SendOtpEmailAsync(email, otp, purpose.ToString());
            if (!emailSent)
            {
                _logger.LogError("Failed to send OTP email to {Email}", email);
                return new OtpResponse
                {
                    Success = false,
                    Message = "Không thể gửi email. Vui lòng thử lại sau"
                };
            }
            _logger.LogInformation($"OTP sent successfully to {email} for {purpose}");

            return new OtpResponse
            {
                Success = true,
                Message = "Mã OTP đã được gửi đến email của bạn",
                ExpiresInSeconds = (int)expiry.TotalSeconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending OTP to {email}");
            return new OtpResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi. Vui lòng thử lại"
            };
        }
    }

    public async Task<OtpResponse> VerifyOtpAsync(string email, string otp, OtpPurpose purpose)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            {
                return new OtpResponse()
                {
                    Success = false,
                    Message = "Email hoặc OTP không hợp lệ"
                };
            }

            var otpKey = GetOtpKey(email, purpose);
            var otpJson = await _redisService.GetStringAsync(otpKey);

            if (string.IsNullOrWhiteSpace(otpJson))
            {
                return new OtpResponse
                {
                    Success = false,
                    Message = "OTP không tồn tại hoặc đã hết hạn"
                };
            }

            var otpData = JsonSerializer.Deserialize<OtpData>(otpJson);
            if (otpData is null)
            {
                return new OtpResponse
                {
                    Success = false,
                    Message = "Dữ liệu OTP không hợp lệ"
                };
            }

            if (otpData.IsVerified)
            {
                return new OtpResponse
                {
                    Success = false,
                    Message = "OTP này đã được sử dụng"
                };
            }

            if (DateTime.UtcNow > otpData.ExpiresAt)
            {
                await _redisService.DeleteAsync(otpKey);
                return new OtpResponse
                {
                    Success = false,
                    Message = "OTP đã hết hạn"
                };
            }

            var attemptsKey = GetAttemptsKey(email, purpose);
            var attemptsStr = await _redisService.GetStringAsync(attemptsKey);
            var attempts = string.IsNullOrWhiteSpace(attemptsStr) ? 0 : int.Parse(attemptsStr);

            if (attempts >= _otpSettings.MaxAttempts)
            {
                await _redisService.DeleteAsync(otpKey);
                return new OtpResponse
                {
                    Success = false,
                    Message = "Bạn đã nhập sai quá số lần cho phép. Vui lòng yêu cầu OTP mới"
                };
            }

            if (otpData.Otp != otp.Trim())
            {
                attempts++;
                await _redisService.SetStringAsync(attemptsKey, attempts.ToString(),
                    TimeSpan.FromMinutes(_otpSettings.ExpiryMinutes));
                var remainingAttempts = _otpSettings.MaxAttempts - attempts;
                return new OtpResponse()
                {
                    Success = false,
                    Message = $"OTP không chính xác. Còn {remainingAttempts} lần thử",
                    RemainingAttempts = remainingAttempts
                };
            }

            otpData.IsVerified = true;
            otpData.VerifiedAt = DateTime.UtcNow;
            var updatedOtpJson = JsonSerializer.Serialize(otpData);
            await _redisService.SetStringAsync(otpKey, updatedOtpJson, TimeSpan.FromMinutes(10));
            await _redisService.DeleteAsync(attemptsKey);
            _logger.LogInformation($"OTP verified successfully for {email} for {purpose}");
            return new OtpResponse()
            {
                Success = true,
                Message = "Xác thực OTP thành công"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error verifying OTP for {email}");
            return new OtpResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi. Vui lòng thử lại"
            };
        }
    }

    public async Task<bool> InvalidateOtpAsync(string email, OtpPurpose purpose)
    {
        try
        {
            var otpKey = GetOtpKey(email, purpose);
            var attemptsKey = GetAttemptsKey(email, purpose);
            var cooldownKey = GetCooldownKey(email, purpose);

            await _redisService.DeleteAsync(otpKey);
            await _redisService.DeleteAsync(attemptsKey);
            await _redisService.DeleteAsync(cooldownKey);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating OTP for {Email}", email);
            return false;
        }
    }

    public async Task<int?> GetRemainingCooldownAsync(string email, OtpPurpose purpose)
    {
        try
        {
            var cooldownKey = GetCooldownKey(email, purpose);
            var cooldownStr = await _redisService.GetStringAsync(cooldownKey);

            if (string.IsNullOrEmpty(cooldownStr))
                return null;

            var cooldownStart = DateTime.Parse(cooldownStr);
            var elapsed = (DateTime.UtcNow - cooldownStart).TotalSeconds;
            var remaining = _otpSettings.ResendCooldownSeconds - (int)elapsed;

            return remaining > 0 ? remaining : null;
        }
        catch
        {
            return null;
        }
    }
    
    private string GenerateOtp(int length)
    {
        const string chars = "0123456789";
        var otp = new char[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            var data = new byte[4 * length];
            rng.GetBytes(data);

            for (var i = 0; i < length; i++)
            {
                var value = BitConverter.ToUInt32(data, i * 4);
                otp[i] = chars[(int)(value % chars.Length)];
            }
        }

        return new string(otp);
    }

    
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    private string GetOtpKey(string email, OtpPurpose purpose)
        => $"{OtpPrefix}{email.ToLower()}:{purpose}";

    private string GetCooldownKey(string email, OtpPurpose purpose)
        => $"{CooldownPrefix}{email.ToLower()}:{purpose}";

    private string GetAttemptsKey(string email, OtpPurpose purpose)
        => $"{AttemptsPrefix}{email.ToLower()}:{purpose}";
}