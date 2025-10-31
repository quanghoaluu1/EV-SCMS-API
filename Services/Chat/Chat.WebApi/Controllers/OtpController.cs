using Chat.Application.IServices;
using Chat.Domain.DTOs;
using Chat.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Chat.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otpService;
    private readonly ILogger<OtpController> _logger;
    public OtpController(IOtpService otpService, ILogger<OtpController> logger)
    {
        _otpService = otpService;
        _logger = logger;
    }

    [HttpPost("send-otp")]
    [ProducesResponseType(typeof(OtpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(400)]
    [ProducesResponseType(429)]
    public async Task<ActionResult<OtpResponse>> SendOtp([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new OtpResponse()
            {
                Success = false,
                Message = "Email không được để trống"
            });
        }
        var result = await _otpService.SendOtpAsync(request.Email, request.Purpose);
        if (!result.Success && result.CooldownSeconds.HasValue)
        {
            return StatusCode(429, result);
        }
        return Ok(result);
    }
    
    [HttpPost("verify")]
    [ProducesResponseType(typeof(OtpResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OtpResponse>> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
        {
            return BadRequest(new OtpResponse
            {
                Success = false,
                Message = "Email và OTP không được để trống"
            });
        }

        var result = await _otpService.VerifyOtpAsync(request.Email, request.Otp, request.Purpose);
        
        return Ok(result);
    }
    
    [HttpPost("invalidate")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult> InvalidateOtp([FromBody] SendOtpRequest request)
    {
        var success = await _otpService.InvalidateOtpAsync(request.Email, request.Purpose);
        
        if (success)
        {
            return Ok(new { message = "OTP đã được hủy" });
        }

        return BadRequest(new { message = "Không thể hủy OTP" });
    }

    [HttpGet("cooldown")]
    [ProducesResponseType(200)]
    public async Task<ActionResult> GetCooldown(
        [FromQuery] string email,
        [FromQuery] OtpPurpose purpose = OtpPurpose.Registration)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { message = "Email không được để trống" });
        }

        var cooldown = await _otpService.GetRemainingCooldownAsync(email, purpose);

        if (cooldown.HasValue)
        {
            return Ok(new
            {
                hasCooldown = true,
                remainingSeconds = cooldown.Value,
                message = $"Vui lòng đợi {cooldown.Value} giây"
            });
        }

        return Ok(new
        {
            hasCooldown = false,
            remainingSeconds = 0,
            message = "Bạn có thể gửi OTP"
        });
    }
}