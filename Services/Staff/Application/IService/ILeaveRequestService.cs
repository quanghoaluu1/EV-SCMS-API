using Application.DTOs;
using Domain.Enum;
using GrpcContract;

namespace Application.IService;

public interface ILeaveRequestService
{
    Task<ResultDto<LeaveRequestDto>> GetByIdAsync(Guid id);
    Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetAllAsync();
    Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetByStaffAsync(Guid staffId);
    Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetByStatusAsync(LeaveRequestStatus status);
    Task<ResultDto<IEnumerable<LeaveRequestDto>>> GetPendingAsync();
    Task<ResultDto<LeaveRequestDto>> CreateAsync(CreateLeaveRequestDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateLeaveRequestDto dto);
    Task<bool> ApproveAsync(Guid id, ApproveLeaveRequestDto dto);
    Task<bool> DeleteAsync(Guid id);
}


