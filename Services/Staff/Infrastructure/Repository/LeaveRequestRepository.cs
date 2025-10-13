using Domain.Entities;
using Domain.Enum;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly StaffDbContext _context;

    public LeaveRequestRepository(StaffDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest?> GetByIdAsync(Guid id)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Staff)
            .FirstOrDefaultAsync(lr => lr.Id == id);
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Staff)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetByStaffIdAsync(Guid staffId)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Staff)
            .Where(lr => lr.StaffId == staffId)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetByStatusAsync(LeaveRequestStatus status)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Staff)
            .Where(lr => lr.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Staff)
            .Where(lr => lr.Status == LeaveRequestStatus.Pending)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingAsync(Guid staffId, DateOnly start, DateOnly end)
    {
        return await _context.LeaveRequests.AnyAsync(lr => lr.StaffId == staffId &&
            lr.Status != LeaveRequestStatus.Rejected &&
            lr.Status != LeaveRequestStatus.Cancelled &&
            lr.StartDate <= end && start <= lr.EndDate);
    }

    public async Task<IEnumerable<Guid>> GetStaffIdsWithApprovedLeaveInRangeAsync(DateOnly start, DateOnly end)
    {
        return await _context.LeaveRequests
            .Where(lr => lr.Status == LeaveRequestStatus.Approved && lr.StartDate <= end && start <= lr.EndDate)
            .Select(lr => lr.StaffId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest)
    {
        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task UpdateAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Update(leaveRequest);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(id);
        if (leaveRequest != null)
        {
            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.LeaveRequests.AnyAsync(lr => lr.Id == id);
    }
}

