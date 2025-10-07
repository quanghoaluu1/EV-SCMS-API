using AuthGrpc;

namespace Infrastructure.Interfaces;

public interface IAuthGrpcGateway
{ 
    Task<GetUserByIdReply> GetUserById(Guid userId);
}