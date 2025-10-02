using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Maintenance.Application.DTOs.MaintenanceDTOs;

namespace Maintenance.Application.IServices
{
    public interface IMaintenanceService
    {
        Task<MaintenanceResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<MaintenanceResponseDto>> GetAllAsync();
        Task<MaintenanceResponseDto> CreateAsync(CreateMaintenanceDto dto);
        Task<MaintenanceResponseDto?> UpdateAsync(Guid id, UpdateMaintenanceDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
