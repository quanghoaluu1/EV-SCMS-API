using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle.Application.DTOs;

namespace Vehicle.Application.IServices
{
    public interface IVehicleService
    {
        Task<VehicleDto> CreateAsync(VehicleCreateDto dto);
        Task<VehicleDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<VehicleDto>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<VehicleDto>> GetAllAsync();
        Task<VehicleDto?> UpdateAsync(Guid id, VehicleUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
