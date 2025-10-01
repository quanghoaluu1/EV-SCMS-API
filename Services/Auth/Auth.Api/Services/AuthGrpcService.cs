using Grpc.Core;
using AuthService.Grpc;
using Auth.Application.IServices;
using Auth.Application.DTOs;

namespace Auth.Api.Services
{
    public class AuthGrpcService : AuthService.Grpc.AuthService.AuthServiceBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthGrpcService> _logger;

        public AuthGrpcService(
            IAuthService authService,
            ILogger<AuthGrpcService> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public override async Task<LoginResponse> Login(AuthService.Grpc.LoginRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                var loginRequest = new Auth.Application.DTOs.LoginRequest
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _authService.LoginAsync(loginRequest);

                if (!result.IsSuccess)
                {
                    return new LoginResponse
                    {
                        IsSuccess = false,
                        Message = result.Message
                    };
                }

                return new LoginResponse
                {
                    IsSuccess = true,
                    Message = result.Message,
                    Data = new AuthService.Grpc.AuthResponse
                    {
                        User = new AuthService.Grpc.User
                        {
                            Email = result.Data.User.Email,
                            FullName = result.Data.User.FullName,
                            PhoneNumber = result.Data.User.PhoneNumber ?? string.Empty,
                            Status = result.Data.User.Status,
                            Role = (int)result.Data.User.Role,
                            CreatedAt = result.Data.User.CreatedAt.Ticks
                        },
                        Token = result.Data.Token,
                        ExpiresAt = result.Data.ExpiresAt.Ticks
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<RegisterResponse> Register(AuthService.Grpc.RegisterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

                var registerRequest = new Auth.Application.DTOs.RegisterRequest
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    Password = request.Password,
                    ConfirmPassword = request.ConfirmPassword,
                };

                var result = await _authService.RegisterAsync(registerRequest);

                if (!result.IsSuccess)
                {
                    return new RegisterResponse
                    {
                        IsSuccess = false,
                        Message = result.Message
                    };
                }

                return new RegisterResponse
                {
                    IsSuccess = true,
                    Message = result.Message,
                    Data = new AuthService.Grpc.AuthResponse
                    {
                        User = new AuthService.Grpc.User
                        {
                            Email = result.Data.User.Email,
                            FullName = result.Data.User.FullName,
                            PhoneNumber = result.Data.User.PhoneNumber ?? string.Empty,
                            Status = result.Data.User.Status,
                            Role = (int)result.Data.User.Role,
                            CreatedAt = result.Data.User.CreatedAt.Ticks
                        },
                        Token = result.Data.Token,
                        ExpiresAt = result.Data.ExpiresAt.Ticks
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Token validation request");

                // You'll need to implement ValidateTokenAsync in your IAuthService
                // For now, this is a placeholder implementation

                // Parse JWT token manually
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

                if (!handler.CanReadToken(request.Token))
                {
                    return new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Invalid token format"
                    };
                }

                var jwtToken = handler.ReadJwtToken(request.Token);

                // Check if token is expired
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Token has expired"
                    };
                }

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                var roleString = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

                // Convert role string to int
                int role = 0;
                if (!string.IsNullOrEmpty(roleString) && Enum.TryParse<Auth.Domain.Enums.RoleEnum>(roleString, out var roleEnum))
                {
                    role = (int)roleEnum;
                }

                return new ValidateTokenResponse
                {
                    IsValid = true,
                    Message = "Token is valid",
                    UserId = userId ?? string.Empty,
                    Email = email ?? string.Empty,
                    Role = role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return new ValidateTokenResponse
                {
                    IsValid = false,
                    Message = "Token validation failed"
                };
            }
        }

        public override async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting user by email: {Email}", request.Email);

                // You'll need to add this method to your IUserRepository
                var user = await _authService.GetUserByEmailAsync(request.Email);

                if (user == null)
                {
                    return new GetUserByEmailResponse
                    {
                        IsSuccess = false,
                        Message = $"User with email {request.Email} not found"
                    };
                }

                return new GetUserByEmailResponse
                {
                    User = new AuthService.Grpc.User
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber ?? string.Empty,
                        Status = user.Status,
                        Role = (int)user.Role,
                        CreatedAt = user.CreatedAt.Ticks
                    },
                    IsSuccess = true,
                    Message = "User retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", request.Email);
                return new GetUserByEmailResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                };
            }
        }
    }
}