using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.IRepository
{
    public interface IInventoryRepository
    {
        Task<Domain.Entities.Inventory> AddAsync(Domain.Entities.Inventory inventory);
        Task<Domain.Entities.Inventory?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Inventory>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<Domain.Entities.Inventory?> UpdateAsync(Domain.Entities.Inventory inventory);
        Task<bool> DeleteAsync(Guid id);

        Task<int> GetTotalCountAsync();

        Task<IEnumerable<Domain.Entities.Inventory>> GetByCategoryAsync(InventoryCategoryEnum category);
        Task<IEnumerable<Domain.Entities.Inventory>> GetByStatusAsync(InventoryStatusEnum status);

        Task<IEnumerable<Domain.Entities.Inventory>> GetLowStockItemsAsync();
        Task<IEnumerable<Domain.Entities.Inventory>> GetOutOfStockItemsAsync();

        Task<bool> AdjustStockAsync(Guid id, int quantityChange);
    }
}
