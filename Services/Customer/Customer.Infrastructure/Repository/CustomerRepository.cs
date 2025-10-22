using Customer.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerServiceDbContext _context;

        public CustomerRepository(CustomerServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Customer> AddAsync(Domain.Entities.Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Domain.Entities.Customer?> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Domain.Entities.Customer?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<List<Domain.Entities.Customer>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Customers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Customer?> UpdateAsync(Domain.Entities.Customer customer)
        {
            _context.Customers.Update(customer);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? customer : null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await GetByIdAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Customer>> GetByStatusAsync(string status, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Customers
                .Where(c => c.Status == status)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid customerId, string newStatus)
        {
            var customer = await GetByIdAsync(customerId);
            if (customer == null) return false;

            customer.Status = newStatus;
            customer.UpdatedAt = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.Customers.AnyAsync(c => c.UserId == userId);
        }
    }
}
