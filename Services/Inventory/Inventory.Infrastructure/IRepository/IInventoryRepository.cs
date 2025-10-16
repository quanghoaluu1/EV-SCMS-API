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
        Task<IEnumerable<Domain.Entities.Inventory>> GetAllAsync();

        Task<Domain.Entities.Inventory?> GetByIdAsync(Guid id);

        Task AddAsync(Domain.Entities.Inventory inventory);

        Task UpdateAsync(Domain.Entities.Inventory inventory);

        Task DeleteAsync(Guid id);

        Task<IEnumerable<Domain.Entities.Inventory>> GetByCategoryAsync(InventoryCategoryEnum category);
    }
}
