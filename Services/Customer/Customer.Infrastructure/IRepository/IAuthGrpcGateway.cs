using AuthGrpc;

namespace Customer.Infrastructure.IRepository;

public interface IAuthGrpcGateway
{
    Task<GetUserByIdReply> GetUserById(Guid userId);

}