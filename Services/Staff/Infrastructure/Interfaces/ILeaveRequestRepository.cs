using Domain.Entities;
using Domain.Enum;

namespace Infrastructure.Interfaces;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<LeaveRequest>> GetAllAsync();
    Task<IEnumerable<LeaveRequest>> GetByStaffIdAsync(Guid staffId);
    Task<IEnumerable<LeaveRequest>> GetByStatusAsync(LeaveRequestStatus status);
    Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync();
    Task<bool> HasOverlappingAsync(Guid staffId, DateOnly start, DateOnly end);
    Task<IEnumerable<Guid>> GetStaffIdsWithApprovedLeaveInRangeAsync(DateOnly start, DateOnly end);
    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task UpdateAsync(LeaveRequest leaveRequest);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

