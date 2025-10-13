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
        Task<InventoryResponseDto> CreateAsync(CreateInventoryDto dto);
        Task<InventoryResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<InventoryListDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<InventoryResponseDto?> UpdateAsync(Guid id, UpdateInventoryDto dto);
        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<InventoryListDto>> GetByCategoryAsync(InventoryCategoryEnum category);
        Task<IEnumerable<InventoryListDto>> GetByStatusAsync(InventoryStatusEnum status);

        Task<InventoryAlertDto> GetAlertsAsync();
        Task<IEnumerable<InventoryListDto>> GetLowStockItemsAsync();
        Task<IEnumerable<InventoryListDto>> GetOutOfStockItemsAsync();
        Task<InventorySummaryDto> GetSummaryAsync();

        Task<bool> AdjustStockAsync(Guid id, AdjustStockDto dto);
    }
}
