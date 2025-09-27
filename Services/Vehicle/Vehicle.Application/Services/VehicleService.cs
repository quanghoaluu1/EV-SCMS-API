using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle.Application.DTOs;
using Vehicle.Application.IServices;
using Vehicle.Infrashtructure.IRepository;

namespace Vehicle.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<VehicleDto> CreateAsync(VehicleCreateDto dto)
        {
            var vehicle = dto.Adapt<Domain.Entities.Vehicle>();
            await _vehicleRepository.AddAsync(vehicle);
            return vehicle.Adapt<VehicleDto>();
        }

        public async Task<VehicleDto?> GetByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle?.Adapt<VehicleDto>();
        }

        public async Task<IEnumerable<VehicleDto>> GetByCustomerIdAsync(Guid customerId)
        {
            var vehicles = await _vehicleRepository.GetByCustomerIdAsync(customerId);
            return vehicles.Adapt<IEnumerable<VehicleDto>>();
        }

        public async Task<IEnumerable<VehicleDto>> GetAllAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Adapt<IEnumerable<VehicleDto>>();
        }

        public async Task<VehicleDto?> UpdateAsync(Guid id, VehicleUpdateDto dto)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            dto.Adapt(existing); // map update fields
            await _vehicleRepository.UpdateAsync(existing);

            return existing.Adapt<VehicleDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _vehicleRepository.DeleteAsync(id);
        }
    }
}
