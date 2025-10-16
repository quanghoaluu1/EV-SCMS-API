using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IStaffShiftRepository
{
    Task<StaffShift?> GetByIdAsync(Guid id);
    Task<IEnumerable<StaffShift>> GetAllAsync();
    Task<IEnumerable<StaffShift>> GetByStaffIdAsync(Guid staffId);
    Task<IEnumerable<StaffShift>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<IEnumerable<StaffShift>> GetByStaffIdAndDateRangeAsync(Guid staffId, DateOnly startDate, DateOnly endDate);
    Task<bool> ExistsForStaffOnDateAsync(Guid staffId, DateOnly workDate);
    Task<StaffShift> AddAsync(StaffShift staffShift);
    Task UpdateAsync(StaffShift staffShift);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

