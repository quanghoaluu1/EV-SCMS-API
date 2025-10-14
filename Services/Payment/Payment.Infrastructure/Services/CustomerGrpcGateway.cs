using CustomerGrpc;
using Payment.Infrastructure.Interfaces;

namespace Payment.Infrastructure.Services;

public class CustomerGrpcGateway : ICustomerGrpcGateway
{
    private readonly CustomerService.CustomerServiceClient _client;
    public CustomerGrpcGateway(CustomerGrpc.CustomerService.CustomerServiceClient client)
    {
        _client = client;
    }
    
    public async Task<GetCustomerByIdReply> GetCustomerById(Guid customerId)
    {
        var request = new GetCustomerByIdRequest
        {
            CustomerId = customerId.ToString()
        };
        return await _client.GetCustomerByIdAsync(request);
    }
}