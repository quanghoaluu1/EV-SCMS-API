using Customer.Application.DTOs;
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
                    CustomerId = customer.Data.Id.ToString(),
                    UserId = customer.Data.UserId.ToString(),
                    Address = customer.Data.Address,
                    Status = customer.Data.Status.ToString(),
                    CreatedAt = customer.Data.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    UpdatedAt = customer.Data.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Customer gRPC exception: {ex}");
                return new GetCustomerByIdReply { Success = false, Message = ex.Message };
            }
        }

        public override async Task<CreateCustomerReply> CreateCustomer(CreateCustomerRequest request,
            ServerCallContext context)
        {
            var customerCreateDto = new CustomerDTOs.CustomerCreateDto()
            {
                UserId = Guid.Parse(request.UserId),
                Address = request.Address
            };
            var customer = await _customerService.CreateAsync(customerCreateDto);
            return new CreateCustomerReply
            {
                Success = true,
                Message = "Customer created",
                CustomerId = customer.Id.ToString(),
                UserId = customer.UserId.ToString()
            };
        }
    }
}
