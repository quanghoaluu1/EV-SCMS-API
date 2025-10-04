using Application.DTOs;

namespace Application.IService;

public interface IStaffService
{
    
    Task<StaffDto?> GetStaffByIdAsync(Guid id);
    Task<IEnumerable<StaffDto>> GetAllStaffAsync();
    Task<StaffDto> CreateStaffAsync(CreateStaffDto dto);
    Task<bool> UpdateStaffAsync(Guid id, UpdateStaffDto dto);
    Task<bool> DeleteStaffAsync(Guid id);
}