using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Application.DTOs;
using Auth.Application.DTOs.Auth;
using Auth.Application.IServices;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Infrastructure;
using Auth.Infrastructure.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IStaffGrpcGateway _staffGrpc;

    public AuthService(IUserRepository userService, IPasswordHasher<User> passwordHasher, IConfiguration configuration,
        IStaffGrpcGateway staffGrpc)
    {
        _userRepository = userService;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _staffGrpc = staffGrpc;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (request.Password != request.ConfirmPassword)
            {
                return new Result<AuthResponse>()
                {
                    IsSuccess = false,
                    Message = "Password and Confirm Password do not match"
                };
            }


            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new Result<AuthResponse>()
                {
                    IsSuccess = false,
                    Message = "Email is required"
                };
            }

            if (await _userRepository.ExistsByEmail(request.Email))
            {
                throw new Exception("Email already exists");
            }

            request.Role ??= RoleEnum.Customer;

            var user = request.Adapt<User>();
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            var createdUser = await _userRepository.CreateUser(user);
            var userDto = createdUser.Adapt<UserDto>();
            var token = GenerateToken(createdUser);

            return new Result<AuthResponse>()
            {
                IsSuccess = true,
                Message = "User registered successfully",
                Data = new AuthResponse()
                {
                    User = userDto,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(2)
                }

            };
        }
        catch (Exception ex)
        {
            return new Result<AuthResponse>()
            {
                IsSuccess = false,
                Message = $"An error occurred during registration: {ex.Message}"
            };
        }
    }

    public async Task<Result<RegisterStaffResponse>> RegisterStaffAsync(RegisterStaffRequest request,
        CancellationToken ct)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return new Result<RegisterStaffResponse>()
            {
                IsSuccess = false,
                Message = "Password and Confirm Password do not match"
            };
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return new Result<RegisterStaffResponse>()
            {
                IsSuccess = false,
                Message = "Email is required"
            };
        }

        if (await _userRepository.ExistsByEmail(request.Email))
        {
            return new Result<RegisterStaffResponse>()
            {
                IsSuccess = false,
                Message = "Email already exists"
            };
        }

        if (request.Role == RoleEnum.Customer)
        {
            return new Result<RegisterStaffResponse>()
            {
                IsSuccess = false,
                Message = "Invalid role for staff registration"
            };
        }

        try
        {
            var user = request.Adapt<User>();
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            var createdUser = await _userRepository.CreateUser(user);
            var userDto = createdUser.Adapt<UserDto>();

            var staffReply = await _staffGrpc.CreateStaffAsync(createdUser.Id, request.HireDate, ct);
            if (!staffReply.Success)
            {
                return new Result<RegisterStaffResponse>()
                {
                    IsSuccess = false,
                    Message = $"Staff creation failed: {staffReply.Message}"
                };
            }

            return new Result<RegisterStaffResponse>()
            {
                IsSuccess = true,
                Message = "Staff registered successfully",
                Data = new RegisterStaffResponse()
                {
                    Auth = new AuthResponse()
                    {
                        User = userDto,
                        Token = GenerateToken(createdUser),
                        ExpiresAt = DateTime.UtcNow.AddHours(2)
                    },
                }
            };
        }
        catch (Exception ex)
        {
            return new Result<RegisterStaffResponse>()
            {
                IsSuccess = false,
                Message = $"An error occurred during staff registration: {ex.Message}"
            };
        }
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                return new Result<AuthResponse>()
                {
                    IsSuccess = false,
                    Message = "Invalid username or password."
                };
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return new Result<AuthResponse>()
                {
                    IsSuccess = false,
                    Message = "Invalid username or password."
                };
            }

            var userDto = user.Adapt<UserDto>();
            var token = GenerateToken(user);

            return new Result<AuthResponse>()
            {
                IsSuccess = true,
                Message = "Login successful",
                Data = new AuthResponse()
                {
                    User = userDto,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(2),
                }
            };
        }
        catch (Exception ex)
        {
            return new Result<AuthResponse>()
            {
                IsSuccess = false,
                Message = $"An error occurred during login: {ex.Message}"
            };
        }
    }

    public async Task<Result<bool>> ChangeRoleAsync(ChangeRoleRequest request)
    {
        try
        {
            var success = await _userRepository.ChangeRole(request.UserId, request.Role);
            if (!success)
            {
                return new Result<bool>()
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Data = false
                };
            }

            return new Result<bool>()
            {
                IsSuccess = true,
                Message = "User role updated successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new Result<bool>()
            {
                IsSuccess = false,
                Message = $"An error occurred while changing role: {ex.Message}",
                Data = false
            };
        }
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return new Result<UserDto>()
                {
                    IsSuccess = false,
                    Message = "User id can not be null",
                    Data = null
                };
            }

            var user = await _userRepository.GetUserById(userId);
            if (user is null)
            {
                return new Result<UserDto>()
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Data = null
                };
            }

            var userDto = user.Adapt<UserDto>();
            return new Result<UserDto>()
            {
                IsSuccess = true,
                Message = "User found",
                Data = userDto
            };
        }
        catch (Exception ex)
        {
            return new Result<UserDto>()
            {
                IsSuccess = false,
                Message = $"An error occurred while getting user: {ex.Message}",
                Data = null
            };
        }
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}