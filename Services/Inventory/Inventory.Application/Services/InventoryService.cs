using Inventory.Application.DTOs;
using Inventory.Application.IServices;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Infrastructure.IRepository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<InventoryDto> CreateAsync(CreateInventoryDto dto)
        {
            var inventory = dto.Adapt<Domain.Entities.Inventory>();
            await _inventoryRepository.AddAsync(inventory);
            return inventory.Adapt<InventoryDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null) return false;

            await _inventoryRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<InventoryDto>> GetAllAsync()
        {
            var inventories = await _inventoryRepository.GetAllAsync();
            return inventories.Adapt<IEnumerable<InventoryDto>>();
        }

        public async Task<IEnumerable<InventoryDto>> GetByCategoryAsync(InventoryCategoryEnum category)
        {
            var inventories = await _inventoryRepository.GetByCategoryAsync(category);
            return inventories.Adapt<IEnumerable<InventoryDto>>();
        }

        public async Task<InventoryDto?> GetByIdAsync(Guid id)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            return inventory?.Adapt<InventoryDto>();
        }

        public async Task<InventoryDto?> UpdateAsync(Guid id, UpdateInventoryDto dto)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null) return null;

            dto.Adapt(inventory); // Map UpdateDto vào entity
            await _inventoryRepository.UpdateAsync(inventory);

            return inventory.Adapt<InventoryDto>();
        }
    }
}
