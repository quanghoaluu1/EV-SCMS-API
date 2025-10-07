using Infrastructure.IRepository;
using AuthGrpc;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class AuthGrpcGateway : IAuthGrpcGateway
{
  private readonly AuthGrpc.Auth.AuthClient _client;
  public AuthGrpcGateway(AuthGrpc.Auth.AuthClient client)
  {
    _client = client;
  }
  public async Task<GetUserByIdReply> GetUserById(Guid userId)
  {
    var request = new GetUserByIdRequest()
    {
      UserId = userId.ToString()
    };
    return await _client.GetUserByIdAsync(request);
  }
}