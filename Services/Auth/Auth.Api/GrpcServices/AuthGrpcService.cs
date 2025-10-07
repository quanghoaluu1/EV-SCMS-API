
using Auth.Application.IServices;
using AuthGrpc;
using Grpc.Core;

namespace Auth.Api.GrpcServices;

public class AuthGrpcService : AuthGrpc.Auth.AuthBase
{
    private readonly IAuthService _authService;
    public AuthGrpcService(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task<GetUserByIdReply> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new GetUserByIdReply() { Success = false, Message = "Invalid UserId" };
            }
            var response = await _authService.GetUserByIdAsync(userId);

            return new GetUserByIdReply()
            {
                Success = true,
                Message = "User found",
                UserId = response.Data.Id.ToString(),
                Email = response.Data.Email,
                FullName = response.Data.FullName,
                Role = response.Data.Role.ToString(),
                Status = response.Data.Status,
                CreatedAt = response.Data.CreatedAt.ToString()
            };
        }
        catch (Exception ex)
        {
            return new GetUserByIdReply()
            {
                Success = false,
                Message = $"Internal server error: {ex.Message}"
            };
        }
    }
}
    
