using Auth.Application.DTOs;
using Auth.Application.DTOs.Auth;
using Auth.Application.Interfaces;
using Auth.Domain.Enums;
using Auth.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IStaffGrpcGateway _staffGrpcGateway;
    public AuthController(IAuthService authService, IStaffGrpcGateway staffGrpcGateway)
    {
        _authService = authService;
        _staffGrpcGateway = staffGrpcGateway;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("register-staff")]
    public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffRequest request)
    {
        if (request.Role == RoleEnum.Customer)
        {
            return BadRequest(new { Message = "Role must be a staff role, not Customer." });
        }

        var userRegister = new RegisterRequest()
        {
            Email = request.Email,
            FullName = request.FullName,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            Role = request.Role
        };
        var response = await _authService.RegisterStaffAsync(request);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("change-role")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleRequest request)
    {
        var result = await _authService.ChangeRoleAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await _authService.GetUserByIdAsync(userId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}