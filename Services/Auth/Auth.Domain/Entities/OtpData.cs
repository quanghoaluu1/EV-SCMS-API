using Auth.Domain.Enums;

namespace Auth.Domain.Entities;

public class OtpData
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public int Attempts { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
}