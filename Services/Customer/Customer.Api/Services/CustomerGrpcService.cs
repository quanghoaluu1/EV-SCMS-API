using CustomerGrpc;
using Customer.Application.IServices;
using Grpc.Core;

namespace Customer.Api.Services
{
    public class CustomerGrpcService : CustomerGrpc.CustomerService.CustomerServiceBase
    {
        private readonly ICustomerService _customerService;

        public CustomerGrpcService(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public override async Task<GetCustomerByIdReply> GetCustomerById(GetCustomerByIdRequest request, ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    return new GetCustomerByIdReply { Success = false, Message = "Invalid CustomerId format" };
                }

                var customer = await _customerService.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new GetCustomerByIdReply { Success = false, Message = "Customer not found" };
                }

                return new GetCustomerByIdReply
                {
                    Success = true,
                    Message = "Customer found",
                    CustomerId = customer.Id.ToString(),
                    UserId = customer.UserId.ToString(),
                    Address = customer.Address,
                    Status = customer.Status.ToString(),
                    CreatedAt = customer.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    UpdatedAt = customer.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Customer gRPC exception: {ex}");
                return new GetCustomerByIdReply { Success = false, Message = ex.Message };
            }
        }



    }
}
