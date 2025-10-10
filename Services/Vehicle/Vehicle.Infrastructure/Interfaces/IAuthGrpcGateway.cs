using AuthGrpc;

namespace Vehicle.Infrastructure.Interfaces;

public interface IAuthGrpcGateway
{
    Task<GetUserByIdReply> GetUserById(Guid userId);
}