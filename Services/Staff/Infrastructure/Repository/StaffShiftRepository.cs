using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class StaffShiftRepository : IStaffShiftRepository
{
    private readonly StaffDbContext _context;

    public StaffShiftRepository(StaffDbContext context)
    {
        _context = context;
    }

    public async Task<StaffShift?> GetByIdAsync(Guid id)
    {
        return await _context.StaffShifts
            .Include(ss => ss.Staff)
            .Include(ss => ss.Shift)
            .FirstOrDefaultAsync(ss => ss.Id == id);
    }

    public async Task<IEnumerable<StaffShift>> GetAllAsync()
    {
        return await _context.StaffShifts
            .Include(ss => ss.Staff)
            .Include(ss => ss.Shift)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffShift>> GetByStaffIdAsync(Guid staffId)
    {
        return await _context.StaffShifts
            .Include(ss => ss.Staff)
            .Include(ss => ss.Shift)
            .Where(ss => ss.StaffId == staffId)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffShift>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.StaffShifts
            .Include(ss => ss.Staff)
            .Include(ss => ss.Shift)
            .Where(ss => ss.WordDate >= startDate && ss.WordDate <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffShift>> GetByStaffIdAndDateRangeAsync(Guid staffId, DateOnly startDate, DateOnly endDate)
    {
        return await _context.StaffShifts
            .Include(ss => ss.Staff)
            .Include(ss => ss.Shift)
            .Where(ss => ss.StaffId == staffId && ss.WordDate >= startDate && ss.WordDate <= endDate)
            .ToListAsync();
    }

    public async Task<bool> ExistsForStaffOnDateAsync(Guid staffId, DateOnly workDate)
    {
        return await _context.StaffShifts.AnyAsync(ss => ss.StaffId == staffId && ss.WordDate == workDate);
    }

    public async Task<StaffShift> AddAsync(StaffShift staffShift)
    {
        await _context.StaffShifts.AddAsync(staffShift);
        await _context.SaveChangesAsync();
        return staffShift;
    }

    public async Task UpdateAsync(StaffShift staffShift)
    {
        _context.StaffShifts.Update(staffShift);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var staffShift = await _context.StaffShifts.FindAsync(id);
        if (staffShift != null)
        {
            _context.StaffShifts.Remove(staffShift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.StaffShifts.AnyAsync(ss => ss.Id == id);
    }
}

