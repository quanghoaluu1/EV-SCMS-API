using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(Guid id);
    Task<IEnumerable<Shift>> GetAllAsync();
    Task<Shift> AddAsync(Shift shift);
    Task UpdateAsync(Shift shift);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

