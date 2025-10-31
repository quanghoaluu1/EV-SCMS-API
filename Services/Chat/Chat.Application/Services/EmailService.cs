using Chat.Application.IServices;
using Chat.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Chat.Application.Services;

public class EmailService : IEmailService
{
    private readonly ISendGridClient _client;
    private readonly ILogger<EmailService> _logger;
    private readonly SendGridSettings _settings;

    public EmailService(IOptions<SendGridSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _client = new SendGridClient(_settings.ApiKey);
    }

    public async Task<bool> SendOtpEmailAsync(string toEmail, string otp, string purpose)
    {
        var subject = GetOtpEmailSubject(purpose);
        var htmlContent = GetOtpEmailTemplate(otp, purpose);
        var plainTextContent = GetOtpPlainText(otp, purpose);

        return await SendEmailAsync(toEmail, subject, htmlContent, plainTextContent);
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent,
        string? plainTextContent = null)
    {
        try
        {
            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent ?? "", htmlContent);

            var res = await _client.SendEmailAsync(msg);
            if (res.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Email sent successfully to {toEmail} from {from.Email}");
                return true;
            }

            var body = await res.Body.ReadAsStringAsync();
            _logger.LogError($"Email failed to send to {toEmail} with error: {body}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            return false;
        }
    }

    private string GetOtpPlainText(string otp, string purpose)
    {
        return $"""

                Mã OTP của bạn là: {otp}

                Mã này có hiệu lực trong 5 phút.
                Vui lòng không chia sẻ mã này với bất kỳ ai.

                Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.

                Trân trọng,
                EV-SCMS

                """;
    }

    private string GetOtpEmailTemplate(string otp, string purpose)
    {
        var purposeText = purpose switch
        {
            "Registration" => "hoàn tất đăng ký tài khoản",
            "PasswordReset" => "đặt lại mật khẩu",
            _ => "Xác thực"
        };
        return $$"""

                 <!DOCTYPE html>
                 <html>
                 <head>
                     <meta charset="UTF-8">
                     <meta name="viewport" content="width=device-width, initial-scale=1.0">
                     <title>Mã OTP</title>
                     <style>
                         body {
                             font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                             background-color: #f4f4f4;
                             margin: 0;
                             padding: 0;
                         }
                         .container {
                             max-width: 600px;
                             margin: 40px auto;
                             background-color: #ffffff;
                             border-radius: 10px;
                             box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                             overflow: hidden;
                         }
                         .header {
                             background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                             color: white;
                             padding: 30px;
                             text-align: center;
                         }
                         .header h1 {
                             margin: 0;
                             font-size: 28px;
                         }
                         .content {
                             padding: 40px 30px;
                         }
                         .otp-box {
                             background-color: #f8f9fa;
                             border: 2px dashed #667eea;
                             border-radius: 8px;
                             padding: 30px;
                             text-align: center;
                             margin: 30px 0;
                         }
                         .otp-code {
                             font-size: 36px;
                             font-weight: bold;
                             color: #667eea;
                             letter-spacing: 8px;
                             font-family: 'Courier New', monospace;
                         }
                         .info {
                             color: #6c757d;
                             font-size: 14px;
                             line-height: 1.6;
                             margin: 20px 0;
                         }
                         .warning {
                             background-color: #fff3cd;
                             border-left: 4px solid #ffc107;
                             padding: 15px;
                             margin: 20px 0;
                             border-radius: 4px;
                         }
                         .warning p {
                             margin: 0;
                             color: #856404;
                             font-size: 14px;
                         }
                         .footer {
                             background-color: #f8f9fa;
                             padding: 20px;
                             text-align: center;
                             color: #6c757d;
                             font-size: 12px;
                         }
                         .button {
                             display: inline-block;
                             padding: 12px 30px;
                             background-color: #667eea;
                             color: white;
                             text-decoration: none;
                             border-radius: 5px;
                             margin: 20px 0;
                         }
                     </style>
                 </head>
                 <body>
                     <div class="container">
                         <div class="header">
                             <h1>🔐 Mã Xác Thực OTP</h1>
                         </div>
                         <div class="content">
                             <p>Xin chào,</p>
                             <p>Bạn đã yêu cầu mã OTP để {{purposeText}}. Vui lòng sử dụng mã sau:</p>
                             
                             <div class="otp-box">
                                 <div class="otp-code">{{otp}}</div>
                                 <p style="margin-top: 15px; color: #6c757d; font-size: 14px;">
                                     Mã có hiệu lực trong <strong>5 phút</strong>
                                 </p>
                             </div>

                             <div class="info">
                                 <p><strong>Lưu ý:</strong></p>
                                 <ul>
                                     <li>Mã OTP này chỉ sử dụng một lần</li>
                                     <li>Không chia sẻ mã này với bất kỳ ai</li>
                                     <li>Mã sẽ hết hạn sau 5 phút</li>
                                 </ul>
                             </div>

                             <div class="warning">
                                 <p><strong>⚠️ Cảnh báo bảo mật:</strong> Nếu bạn không yêu cầu mã này, 
                                 vui lòng bỏ qua email này và liên hệ với bộ phận hỗ trợ ngay lập tức.</p>
                             </div>
                         </div>
                         <div class="footer">
                             <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                             <p>&copy; 2025 Car Maintenance Service. All rights reserved.</p>
                         </div>
                     </div>
                 </body>
                 </html>
                 """;
    }

    private string GetOtpEmailSubject(string purpose)
    {
        return purpose switch
        {
            "Registration" => "Xác thực tài khoản - Mã OTP",
            "PasswordReset" => "Đặt lại mật khẩu - Mã OTP",
            _ => "Mã xác thực OTP"
        };
    }
}