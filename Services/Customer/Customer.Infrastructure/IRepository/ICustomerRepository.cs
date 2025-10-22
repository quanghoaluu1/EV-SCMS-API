using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Infrastructure.IRepository
{
    public interface ICustomerRepository
    {
        Task<Domain.Entities.Customer> AddAsync(Domain.Entities.Customer customer);
        Task<Domain.Entities.Customer?> GetByIdAsync(Guid id);
        Task<Domain.Entities.Customer?> GetByUserIdAsync(Guid userId);
        Task<List<Domain.Entities.Customer>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<Domain.Entities.Customer?> UpdateAsync(Domain.Entities.Customer customer);
        Task<bool> DeleteAsync(Guid id);

        // Pagination support
        Task<int> GetTotalCountAsync();

        // Query methods
        Task<IEnumerable<Domain.Entities.Customer>> GetByStatusAsync(string status, int pageNumber = 1, int pageSize = 10);

        // Specific operations
        Task<bool> UpdateStatusAsync(Guid customerId, string newStatus);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}
