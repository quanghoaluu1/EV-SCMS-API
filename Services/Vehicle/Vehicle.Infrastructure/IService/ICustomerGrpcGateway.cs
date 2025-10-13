using CustomerGrpc;

namespace Vehicle.Infrastructure.IService
{
    public interface ICustomerGrpcGateway
    {
        Task<GetCustomerByIdReply> GetCustomerById(Guid customerId);
    }
}
