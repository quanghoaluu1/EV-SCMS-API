using Customer.Application.IServices;
using Customer.Infrastructure.IRepository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Customer.Application.DTOs.CustomerDTOs;

namespace Customer.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
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

        public async Task<CustomerResponseDto?> GetByIdAsync(Guid customerId)
        {
            var customer = await _repository.GetByIdAsync(customerId);
            return customer?.Adapt<CustomerResponseDto>();
        }

        public async Task<CustomerResponseDto?> GetByUserIdAsync(Guid userId)
        {
            var customer = await _repository.GetByUserIdAsync(userId);
            return customer?.Adapt<CustomerResponseDto>();
        }

        public async Task<IEnumerable<CustomerListDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var customers = await _repository.GetAllAsync(pageNumber, pageSize);
            return customers.Adapt<IEnumerable<CustomerListDto>>();
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
