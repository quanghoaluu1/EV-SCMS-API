using Application.DTOs;
using GrpcContract;

namespace Application.IService;

public interface IStaffShiftService
{
    Task<ResultDto<StaffShiftDto>> GetByIdAsync(Guid id);
    Task<ResultDto<IEnumerable<StaffShiftDto>>> GetAllAsync();
    Task<ResultDto<IEnumerable<StaffShiftDto>>> GetByStaffAsync(Guid staffId);
    Task<ResultDto<IEnumerable<StaffShiftDto>>> GetByDateRangeAsync(DateOnly start, DateOnly end);
    Task<ResultDto<IEnumerable<StaffShiftDto>>> GetByStaffAndDateRangeAsync(Guid staffId, DateOnly start, DateOnly end);
    Task<ResultDto<StaffShiftDto>> CreateAsync(CreateStaffShiftDto dto);
    Task<ResultDto<bool>> UpdateAsync(Guid id, UpdateStaffShiftDto dto);
    Task<ResultDto<bool>> DeleteAsync(Guid id);
    Task<ResultDto<IEnumerable<StaffDto>>> GetAvailableStaffAsync(DateOnly start, DateOnly end);
}


