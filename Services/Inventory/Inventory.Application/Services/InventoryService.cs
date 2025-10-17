using Inventory.Application.DTOs;
using Inventory.Application.IServices;
using Inventory.Domain.Enums;
using Inventory.Infrastructure.IRepository;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Inventory.Application.DTOs.InventoryDTOs;

namespace Inventory.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IInventoryRepository repository,
            ILogger<InventoryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        public async Task<InventoryResponseDto> CreateAsync(CreateInventoryDto dto)
        {
            _logger.LogInformation("Creating inventory item: {Name}", dto.Name);

            if (dto.Quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            if (dto.MinimumStockLevel < 1)
                throw new ArgumentException("Minimum stock level must be at least 1");

            var inventory = dto.Adapt<Domain.Entities.Inventory>();
            var created = await _repository.AddAsync(inventory);

            _logger.LogInformation("Inventory item {Id} created successfully", created.Id);
            return created.Adapt<InventoryResponseDto>();
        }

        public async Task<InventoryResponseDto?> GetByIdAsync(Guid id)
        {
            var inventory = await _repository.GetByIdAsync(id);
            return inventory?.Adapt<InventoryResponseDto>();
        }

        public async Task<IEnumerable<InventoryListDto>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var inventories = await _repository.GetAllAsync(pageNumber, pageSize);
            return inventories.Adapt<IEnumerable<InventoryListDto>>();
        }

        public async Task<InventoryResponseDto?> UpdateAsync(Guid id, UpdateInventoryDto dto)
        {
            _logger.LogInformation("Updating inventory item {Id}", id);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Inventory item {Id} not found", id);
                return null;
            }

            if (dto.Quantity.HasValue && dto.Quantity.Value < 0)
                throw new ArgumentException("Quantity cannot be negative");

            if (dto.MinimumStockLevel.HasValue && dto.MinimumStockLevel.Value < 1)
                throw new ArgumentException("Minimum stock level must be at least 1");

            dto.Adapt(existing);
            var updated = await _repository.UpdateAsync(existing);

            _logger.LogInformation("Inventory item {Id} updated successfully", id);
            return updated?.Adapt<InventoryResponseDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting inventory item {Id}", id);

            var result = await _repository.DeleteAsync(id);

            if (result)
                _logger.LogInformation("Inventory item {Id} deleted successfully", id);
            else
                _logger.LogWarning("Inventory item {Id} not found for deletion", id);

            return result;
        }


        public async Task<IEnumerable<InventoryListDto>> GetByCategoryAsync(
            InventoryCategoryEnum category)
        {
            _logger.LogInformation("Getting inventory items by category: {Category}", category);

            var inventories = await _repository.GetByCategoryAsync(category);
            return inventories.Adapt<IEnumerable<InventoryListDto>>();
        }

        public async Task<IEnumerable<InventoryListDto>> GetByStatusAsync(
            InventoryStatusEnum status)
        {
            _logger.LogInformation("Getting inventory items by status: {Status}", status);

            var inventories = await _repository.GetByStatusAsync(status);
            return inventories.Adapt<IEnumerable<InventoryListDto>>();
        }


        public async Task<InventoryAlertDto> GetAlertsAsync()
        {
            var lowStockItems = await GetLowStockItemsAsync();
            var outOfStockItems = await GetOutOfStockItemsAsync();

            return new InventoryAlertDto
            {
                LowStockCount = lowStockItems.Count(),
                OutOfStockCount = outOfStockItems.Count(),
                LowStockItems = lowStockItems,
                OutOfStockItems = outOfStockItems
            };
        }

        public async Task<IEnumerable<InventoryListDto>> GetLowStockItemsAsync()
        {
            _logger.LogInformation("Getting low stock items");

            var items = await _repository.GetLowStockItemsAsync();
            return items.Adapt<IEnumerable<InventoryListDto>>();
        }

        public async Task<IEnumerable<InventoryListDto>> GetOutOfStockItemsAsync()
        {
            _logger.LogInformation("Getting out of stock items");

            var items = await _repository.GetOutOfStockItemsAsync();
            return items.Adapt<IEnumerable<InventoryListDto>>();
        }

        public async Task<InventorySummaryDto> GetSummaryAsync()
        {
            _logger.LogInformation("Getting inventory summary");

            var allItems = await _repository.GetAllAsync(1, int.MaxValue);
            var itemsList = allItems.ToList();

            var lowStockCount = itemsList.Count(x => x.Quantity > 0 && x.Quantity <= x.MinimumStockLevel);
            var outOfStockCount = itemsList.Count(x => x.Quantity <= 0);

            var healthStatus = outOfStockCount > 0 ? "Critical" :
                               lowStockCount > 5 ? "Warning" : "Good";

            return new InventorySummaryDto
            {
                TotalItems = itemsList.Count,
                TotalQuantity = itemsList.Sum(x => x.Quantity),
                TotalValue = itemsList.Sum(x => x.Quantity * x.UnitPrice),
                LowStockCount = lowStockCount,
                OutOfStockCount = outOfStockCount,
                HealthStatus = healthStatus
            };
        }


        public async Task<bool> AdjustStockAsync(Guid id, AdjustStockDto dto)
        {
            _logger.LogInformation("Adjusting stock for inventory {Id} by {Change}",
                id, dto.QuantityChange);

            try
            {
                var result = await _repository.AdjustStockAsync(id, dto.QuantityChange);

                if (result)
                    _logger.LogInformation("Stock adjusted successfully for inventory {Id}", id);
                else
                    _logger.LogWarning("Inventory {Id} not found for stock adjustment", id);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to adjust stock for inventory {Id}", id);
                throw;
            }
        }

    }
}
