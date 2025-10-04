using Maintenance.Application.IServices;
using Maintenance.Infrastructure.IRepository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Maintenance.Application.DTOs.MaintenanceDTOs;

namespace Maintenance.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepository;

        public MaintenanceService(IMaintenanceRepository maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<MaintenanceResponseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _maintenanceRepository.GetByIdAsync(id);
            return entity?.Adapt<MaintenanceResponseDto>();
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetAllAsync()
        {
            var entities = await _maintenanceRepository.GetAllAsync();
            return entities.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<MaintenanceResponseDto> CreateAsync(CreateMaintenanceDto dto)
        {
            var entity = dto.Adapt<Domain.Entities.Maintenance>();
            await _maintenanceRepository.AddAsync(entity);
            await _maintenanceRepository.SaveChangesAsync();

            return entity.Adapt<MaintenanceResponseDto>();
        }

        public async Task<MaintenanceResponseDto?> UpdateAsync(Guid id, UpdateMaintenanceDto dto)
        {
            var entity = await _maintenanceRepository.GetByIdAsync(id);
            if (entity == null) return null;

            dto.Adapt(entity);

            await _maintenanceRepository.UpdateAsync(entity);
            await _maintenanceRepository.SaveChangesAsync();

            return entity.Adapt<MaintenanceResponseDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _maintenanceRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _maintenanceRepository.DeleteAsync(id);
            await _maintenanceRepository.SaveChangesAsync();
            return true;
        }
    }
}
