using Maintenance.Domain.Enums;
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
        // CRUD 
        Task<MaintenanceResponseDto> CreateAsync(CreateMaintenanceDto dto);
        Task<MaintenanceResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<MaintenanceResponseDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<MaintenanceResponseDto?> UpdateAsync(Guid id, UpdateMaintenanceDto dto);
        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<MaintenanceResponseDto>> GetByBookingIdAsync(Guid bookingId);
        Task<IEnumerable<MaintenanceResponseDto>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<MaintenanceResponseDto>> GetByVehicleIdAsync(Guid vehicleId);
        Task<IEnumerable<MaintenanceResponseDto>> GetByStaffIdAsync(Guid staffId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<MaintenanceResponseDto>> GetByStatusAsync(MaintenanceEnums status, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<MaintenanceResponseDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<bool> UpdateStatusAsync(Guid maintenanceId, MaintenanceEnums status);
        Task<bool> AssignStaffAsync(Guid maintenanceId, Guid staffId);
    }
}
