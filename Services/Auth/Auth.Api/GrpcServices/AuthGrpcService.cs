using Auth.Grpc;
using Auth.Application.IServices;
using Grpc.Core;
namespace Auth.Api.GrpcServices;

public class AuthGrpcService : AuthGrpc.AuthGrpcBase
{
    private readonly IAuthService _authService;

    public AuthGrpcService(IAuthService authService)
    {
        _authService = authService;
    }
    
    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await _authService.LoginAsync(new Auth.Application.DTOs.LoginRequest
        {
            Email = request.Email,
            Password = request.Password
        });
        var reply = new LoginReply
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message ?? ""
        };

        if (result.IsSuccess && result.Data != null)
        {
            reply.Token = result.Data.Token;
            reply.ExpiresAt = result.Data.ExpiresAt.ToString("O");
            reply.User = new LoginReply.Types.User
            {
                Id = result.Data.User.Id.ToString(),
                Email = result.Data.User.Email,
                Role = result.Data.User.Role.ToString(),
            };
        }

        return reply;
    }
    public override async Task<LoginReply> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await _authService.RegisterAsync(new Application.DTOs.RegisterRequest
        {
            Email = request.Email,
            FullName = request.Fullname,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
        });
        
        var reply = new LoginReply
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message ?? ""
        };

        if (result.IsSuccess && result.Data != null)
        {
            reply.Token = result.Data.Token;
            reply.ExpiresAt = result.Data.ExpiresAt.ToString("O");
            reply.User = new LoginReply.Types.User
            {
                Id = result.Data.User.Id.ToString(),
                Email = result.Data.User.Email,
                Role = result.Data.User.Role.ToString(),
            };
        }

        return reply;
    }
}