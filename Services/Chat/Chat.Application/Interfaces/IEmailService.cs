namespace Chat.Application.IServices;

public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(string toEmail, string otp, string purpose);
    Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string? plainTextContent = null);
}