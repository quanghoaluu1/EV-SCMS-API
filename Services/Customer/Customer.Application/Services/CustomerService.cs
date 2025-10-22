using Customer.Application.IServices;
using Customer.Infrastructure.IRepository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrpcContract;
using static Customer.Application.DTOs.CustomerDTOs;

namespace Customer.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IAuthGrpcGateway _authGrpcGateway;


        public CustomerService(ICustomerRepository repository, IAuthGrpcGateway authGrpcGateway)
        {
            _repository = repository;
            _authGrpcGateway = authGrpcGateway;
        }

        public async Task<CustomerResponseDto> CreateAsync(CustomerCreateDto dto)
        {
            var customer = dto.Adapt<Domain.Entities.Customer>();
            customer.CreatedAt = DateTime.UtcNow;
            customer.UpdatedAt = DateTime.UtcNow;

            var created = await _repository.AddAsync(customer);
            return created.Adapt<CustomerResponseDto>();
        }

        public async Task<CustomerResponseDto?> UpdateAsync(Guid customerId, CustomerUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(customerId);
            if (existing == null) return null;

            dto.Adapt(existing); // update in-place
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);
            return updated?.Adapt<CustomerResponseDto>();
        }

        public async Task<bool> DeleteAsync(Guid customerId)
        {
            return await _repository.DeleteAsync(customerId);
        }

        public async Task<ResultDto<CustomerResponseDto>> GetByIdAsync(Guid customerId)
        {
            var customer = await _repository.GetByIdAsync(customerId);
            var userReply = await _authGrpcGateway.GetUserById(customer.UserId);
            if (userReply == null || !userReply.Success)
            {
                return new ResultDto<CustomerResponseDto>()
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve user information through gRPC",
                    Data = null,
                };
            }
            var customerDto = customer?.Adapt<CustomerResponseDto>();
            if (customerDto != null)
            {
                customerDto.Email = userReply.Email;
                customerDto.FullName = userReply.FullName;
            }

            return new ResultDto<CustomerResponseDto>()
            {
                IsSuccess = customer != null,
                Message = customer != null ? "Customer retrieved successfully" : "Customer not found",
                Data = customerDto,
            };
        }

        public async Task<CustomerResponseDto?> GetByUserIdAsync(Guid userId)
        {
            var customer = await _repository.GetByUserIdAsync(userId);
            
            return customer?.Adapt<CustomerResponseDto>();
        }

        public async Task<ResultDto<List<CustomerResponseDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var customers = await _repository.GetAllAsync(pageNumber, pageSize);
            var customerDtos = customers.Adapt<List<CustomerResponseDto>>();
            var tasks = customerDtos.Select(async customerDto =>
            {
                var userReply = await _authGrpcGateway.GetUserById(customerDto.UserId);
                Console.WriteLine("User Reply: " + (userReply != null ? userReply.ToString() : "null"));
                if (userReply?.Success == true)
                {
                    customerDto.FullName = userReply.FullName;
                    customerDto.Email = userReply.Email;
                }
            });
            await Task.WhenAll(tasks);

            return new ResultDto<List<CustomerResponseDto>>()
            {
                IsSuccess = true,
                Message = "Customers retrieved successfully",
                Data = customerDtos
            };
        }

        public async Task<IEnumerable<CustomerListDto>> GetByStatusAsync(string status, int pageNumber = 1, int pageSize = 10)
        {
            var customers = await _repository.GetByStatusAsync(status, pageNumber, pageSize);
            return customers.Adapt<IEnumerable<CustomerListDto>>();
        }

        public async Task<bool> UpdateStatusAsync(Guid customerId, string status)
        {
            return await _repository.UpdateStatusAsync(customerId, status);
        }
    }
}
