using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Customer.Application.DTOs.CustomerDTOs;

namespace Customer.Application.IServices
{
    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateAsync(CustomerCreateDto dto);
        Task<CustomerResponseDto?> UpdateAsync(Guid customerId, CustomerUpdateDto dto);
        Task<bool> DeleteAsync(Guid customerId);
        Task<CustomerResponseDto?> GetByIdAsync(Guid customerId);
        Task<CustomerResponseDto?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<CustomerListDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<CustomerListDto>> GetByStatusAsync(string status, int pageNumber = 1, int pageSize = 10);
        Task<bool> UpdateStatusAsync(Guid customerId, string status);
    }
}
