using Application.DTOs;
using GrpcContract;

namespace Application.IService;

public interface IStaffService
{
    
    Task<ResultDto<StaffDto>> GetStaffByIdAsync(Guid id);
    Task<ResultDto<StaffDto>> GetStaffByUserIdAsync(Guid userId);
    Task<ResultDto<IEnumerable<StaffDto>>> GetAllStaffAsync();
    Task<StaffDto> CreateStaffAsync(CreateStaffDto dto);
    Task<bool> UpdateStaffAsync(Guid id, UpdateStaffDto dto);
    Task<bool> DeleteStaffAsync(Guid id);
    Task<ResultDto<List<StaffDto>>> GetAllStaffByRoleAsync(int role);
}