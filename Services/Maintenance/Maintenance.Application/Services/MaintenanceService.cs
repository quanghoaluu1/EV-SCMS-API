using Maintenance.Application.IServices;
using Maintenance.Domain.Enums;
using Maintenance.Infrastructure.IRepository;
using Mapster;
using Microsoft.Extensions.Logging;
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
        private readonly IMaintenanceRepository _repository;
        private readonly ILogger<MaintenanceService> _logger;

        public MaintenanceService(
            IMaintenanceRepository repository,
            ILogger<MaintenanceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // CRUD

        public async Task<MaintenanceResponseDto> CreateAsync(CreateMaintenanceDto dto)
        {
            _logger.LogInformation("Creating maintenance for booking {BookingId}", dto.BookingId);

            // Business validation
            if (dto.ServiceDate < DateTime.UtcNow.Date)
            {
                _logger.LogWarning("Attempted to create maintenance with past date: {ServiceDate}", dto.ServiceDate);
                throw new ArgumentException("Service date cannot be in the past");
            }

            // Store original labor/service cost
            var originalLaborCost = dto.Cost;

            // Auto-calculate inventory item total costs
            if (dto.InventoryItems != null && dto.InventoryItems.Any())
            {
                foreach (var item in dto.InventoryItems)
                {
                    var calculatedTotal = item.Quantity * item.UnitCost;

                    if (item.TotalCost != calculatedTotal)
                    {
                        _logger.LogWarning("Auto-correcting inventory item total cost. Item: {InventoryId}, Provided: {Provided}, Calculated: {Calculated}",
                            item.InventoryId, item.TotalCost, calculatedTotal);

                        item.TotalCost = calculatedTotal;
                    }
                }

                // NEW: Calculate and update total maintenance cost including inventory
                var inventoryTotal = dto.InventoryItems.Sum(item => item.TotalCost);
                var newTotalCost = originalLaborCost + inventoryTotal;

                _logger.LogInformation("Calculating total maintenance cost. Labor: {LaborCost}, Inventory: {InventoryTotal}, Total: {TotalCost}",
                    originalLaborCost, inventoryTotal, newTotalCost);

                dto.Cost = newTotalCost;
            }

            // Map DTO to Entity
            var maintenance = dto.Adapt<Domain.Entities.Maintenance>();

            // Handle inventory items
            if (dto.InventoryItems != null && dto.InventoryItems.Any())
            {
                maintenance.InventoryItems = dto.InventoryItems.Select(item => new Domain.Entities.MaintenanceInventoryItem
                {
                    InventoryId = item.InventoryId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    TotalCost = item.TotalCost
                }).ToList();
            }

            // Repository handles ID and timestamps
            var created = await _repository.AddAsync(maintenance);

            _logger.LogInformation("Maintenance {MaintenanceId} created successfully with total cost {TotalCost}",
                created.MaintenanceId, created.Cost);

            return created.Adapt<MaintenanceResponseDto>();
        }

        public async Task<MaintenanceResponseDto?> GetByIdAsync(Guid id)
        {
            var maintenance = await _repository.GetByIdAsync(id);
            return maintenance?.Adapt<MaintenanceResponseDto>();
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var maintenances = await _repository.GetAllAsync(pageNumber, pageSize);
            return maintenances.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<MaintenanceResponseDto?> UpdateAsync(Guid id, UpdateMaintenanceDto dto)
        {
            _logger.LogInformation("Updating maintenance {MaintenanceId}", id);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Maintenance {MaintenanceId} not found", id);
                return null;
            }

            // Business validation
            if (dto.ServiceDate.HasValue && dto.ServiceDate.Value < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Service date cannot be in the past");
            }

            // Handle cost calculation for updates
            decimal? originalLaborCost = dto.Cost ?? existing.Cost;

            // If updating inventory items, recalculate total cost
            if (dto.InventoryItems != null)
            {
                // Auto-calculate inventory item total costs
                foreach (var item in dto.InventoryItems)
                {
                    var calculatedTotal = item.Quantity * item.UnitCost;

                    if (item.TotalCost != calculatedTotal)
                    {
                        _logger.LogWarning("Auto-correcting inventory item total cost during update. Item: {InventoryId}, Provided: {Provided}, Calculated: {Calculated}",
                            item.InventoryId, item.TotalCost, calculatedTotal);

                        item.TotalCost = calculatedTotal;
                    }
                }

                // NEW: If Cost was not explicitly provided, recalculate it
                if (!dto.Cost.HasValue)
                {
                    // Use existing cost as base labor cost, subtract any existing inventory costs
                    var existingInventoryTotal = existing.InventoryItems?.Sum(item => item.TotalCost) ?? 0;
                    var baseLaborCost = existing.Cost - existingInventoryTotal;

                    // Calculate new total with updated inventory
                    var newInventoryTotal = dto.InventoryItems.Sum(item => item.TotalCost);
                    dto.Cost = baseLaborCost + newInventoryTotal;

                    _logger.LogInformation("Recalculating maintenance cost. Base Labor: {LaborCost}, New Inventory Total: {InventoryTotal}, New Total: {TotalCost}",
                        baseLaborCost, newInventoryTotal, dto.Cost);
                }
                else
                {
                    // Cost was explicitly provided, add inventory total to it
                    var newInventoryTotal = dto.InventoryItems.Sum(item => item.TotalCost);
                    dto.Cost = dto.Cost.Value + newInventoryTotal;

                    _logger.LogInformation("Updating maintenance cost. Provided Labor: {LaborCost}, Inventory Total: {InventoryTotal}, New Total: {TotalCost}",
                        originalLaborCost, newInventoryTotal, dto.Cost);
                }
            }

            dto.Adapt(existing);

            if (dto.InventoryItems != null)
            {
                existing.InventoryItems.Clear();
                existing.InventoryItems = dto.InventoryItems.Select(item => new Domain.Entities.MaintenanceInventoryItem
                {
                    InventoryId = item.InventoryId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    TotalCost = item.TotalCost
                }).ToList();
            }

            var updated = await _repository.UpdateAsync(existing);

            _logger.LogInformation("Maintenance {MaintenanceId} updated successfully with total cost {TotalCost}",
                id, updated?.Cost);

            return updated?.Adapt<MaintenanceResponseDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting maintenance {MaintenanceId}", id);

            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                _logger.LogInformation("Maintenance {MaintenanceId} deleted successfully", id);
            }
            else
            {
                _logger.LogWarning("Maintenance {MaintenanceId} not found for deletion", id);
            }

            return result;
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetByBookingIdAsync(Guid bookingId)
        {
            _logger.LogInformation("Getting maintenance records for booking {BookingId}", bookingId);

            var maintenances = await _repository.GetByBookingIdAsync(bookingId);
            return maintenances.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetByCustomerIdAsync(
            Guid customerId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Getting maintenance history for customer {CustomerId}", customerId);

            var maintenances = await _repository.GetByCustomerIdAsync(customerId, pageNumber, pageSize);
            return maintenances.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetByVehicleIdAsync(Guid vehicleId)
        {
            _logger.LogInformation("Getting maintenance history for vehicle {VehicleId}", vehicleId);

            var maintenances = await _repository.GetByVehicleIdAsync(vehicleId);
            return maintenances.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetByStaffIdAsync(
            Guid staffId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Getting maintenance jobs for staff {StaffId}", staffId);

            var maintenances = await _repository.GetByStaffIdAsync(staffId, pageNumber, pageSize);
            return maintenances.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetByStatusAsync(
            MaintenanceEnums status,
            int pageNumber = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Getting maintenance records with status {Status}", status);

            var maintenances = await _repository.GetByStatusAsync(status, pageNumber, pageSize);
            return maintenances.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<IEnumerable<MaintenanceResponseDto>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            _logger.LogInformation("Getting maintenance records from {StartDate} to {EndDate}",
                startDate, endDate);

            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before end date");
            }

            var maintenances = await _repository.GetByDateRangeAsync(startDate, endDate);
            return maintenances.Adapt<IEnumerable<MaintenanceResponseDto>>();
        }

        public async Task<bool> UpdateStatusAsync(Guid maintenanceId, MaintenanceEnums status)
        {
            _logger.LogInformation("Updating maintenance {MaintenanceId} status to {Status}",
                maintenanceId, status);

            var result = await _repository.UpdateStatusAsync(maintenanceId, status);

            if (result)
            {
                _logger.LogInformation("Maintenance {MaintenanceId} status updated to {Status}",
                    maintenanceId, status);
            }
            else
            {
                _logger.LogWarning("Maintenance {MaintenanceId} not found for status update",
                    maintenanceId);
            }

            return result;
        }

        public async Task<bool> AssignStaffAsync(Guid maintenanceId, Guid staffId)
        {
            _logger.LogInformation("Assigning staff {StaffId} to maintenance {MaintenanceId}",
                staffId, maintenanceId);

            // Optional: Validate staff exists via gRPC call to Staff Service
            // var staffExists = await _staffServiceClient.CheckStaffExistsAsync(staffId);
            // if (!staffExists) throw new ArgumentException("Staff not found");

            var result = await _repository.AssignStaffAsync(maintenanceId, staffId);

            if (result)
            {
                _logger.LogInformation("Staff {StaffId} assigned to maintenance {MaintenanceId}",
                    staffId, maintenanceId);
            }
            else
            {
                _logger.LogWarning("Maintenance {MaintenanceId} not found for staff assignment",
                    maintenanceId);
            }

            return result;
        }

    }
}
