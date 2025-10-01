using ApiGateway.Services;
using Auth.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthGrpcClient _authClient;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthGrpcClient authClient, ILogger<AuthController> logger)
        {
            _authClient = authClient;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _authClient.LoginAsync(request.Email, request.Password);

                if (!response.IsSuccess)
                    return BadRequest(new { message = response.Message });

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var response = await _authClient.RegisterAsync(
                    request.Email,
                    request.Password,
                    request.ConfirmPassword,
                    request.FullName,
                    request.PhoneNumber ?? string.Empty,
                    (int)request.Role);

                if (!response.IsSuccess)
                    return BadRequest(new { message = response.Message });

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequestDto request)
        {
            try
            {
                var response = await _authClient.ValidateTokenAsync(request.Token);

                if (!response.IsValid)
                    return Unauthorized(new { message = response.Message });

                return Ok(new
                {
                    userId = response.UserId,
                    email = response.Email,
                    role = response.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var response = await _authClient.GetUserByEmailAsync(email);

                if (!response.IsSuccess)
                    return NotFound(new { message = response.Message });

                return Ok(response.User);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }

    // Request DTOs
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public RoleEnum Role { get; set; } = RoleEnum.Customer;
    }

    public class ValidateTokenRequestDto
    {
        public string Token { get; set; } = string.Empty;
    }
}