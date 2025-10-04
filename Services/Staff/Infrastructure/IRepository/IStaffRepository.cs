using Domain.Entities;

namespace Infrastructure.IRepository;

public interface IStaffRepository
{
    Task<Staff?> GetByIdAsync(Guid id);
    Task<IEnumerable<Staff>> GetAllAsync();
    Task AddAsync(Staff staff);
    Task UpdateAsync(Staff staff);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}