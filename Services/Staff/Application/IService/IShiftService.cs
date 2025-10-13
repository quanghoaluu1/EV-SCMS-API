using Application.DTOs;

namespace Application.IService;

public interface IShiftService
{
    Task<ShiftDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ShiftDto>> GetAllAsync();
    Task<ShiftDto> CreateAsync(CreateShiftDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateShiftDto dto);
    Task<bool> DeleteAsync(Guid id);
}


