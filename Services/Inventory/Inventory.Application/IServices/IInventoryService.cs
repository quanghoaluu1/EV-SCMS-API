using Inventory.Application.DTOs;
using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Inventory.Application.DTOs.InventoryDTOs;

namespace Inventory.Application.IServices
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllAsync();
        Task<InventoryDto?> GetByIdAsync(Guid id);
        Task<InventoryDto> CreateAsync(CreateInventoryDto dto);
        Task<InventoryDto?> UpdateAsync(Guid id, UpdateInventoryDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<InventoryDto>> GetByCategoryAsync(InventoryCategoryEnum category);
    }
}
