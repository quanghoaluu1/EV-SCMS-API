using Inventory.Domain.Enums;
using Inventory.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryServiceDbContext _context;

        public InventoryRepository(InventoryServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Inventory> AddAsync(Domain.Entities.Inventory inventory)
        {
            inventory.Id = Guid.NewGuid();
            inventory.CreatedAt = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;

            // Auto set status based on quantity
            UpdateInventoryStatus(inventory);

            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Domain.Entities.Inventory?> GetByIdAsync(Guid id)
        {
            return await _context.Inventories
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Domain.Entities.Inventory>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Inventories
                .AsNoTracking()
                .OrderBy(i => i.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Inventory?> UpdateAsync(Domain.Entities.Inventory inventory)
        {
            var existing = await _context.Inventories.FindAsync(inventory.Id);
            if (existing == null) return null;

            existing.Name = inventory.Name;
            existing.Category = inventory.Category;
            existing.Quantity = inventory.Quantity;
            existing.MinimumStockLevel = inventory.MinimumStockLevel;
            existing.UnitPrice = inventory.UnitPrice;
            existing.Status = inventory.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            UpdateInventoryStatus(existing);

            try
            {
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return false;

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Inventories.CountAsync();
        }


        public async Task<IEnumerable<Domain.Entities.Inventory>> GetByCategoryAsync(
            InventoryCategoryEnum category)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Where(i => i.Category == category)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Inventory>> GetByStatusAsync(
            InventoryStatusEnum status)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Where(i => i.Status == status)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }


        public async Task<IEnumerable<Domain.Entities.Inventory>> GetLowStockItemsAsync()
        {
            return await _context.Inventories
                .AsNoTracking()
                .Where(i => i.Quantity > 0 && i.Quantity <= i.MinimumStockLevel)
                .OrderBy(i => i.Quantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Inventory>> GetOutOfStockItemsAsync()
        {
            return await _context.Inventories
                .AsNoTracking()
                .Where(i => i.Quantity <= 0)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<bool> AdjustStockAsync(Guid id, int quantityChange)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return false;

            var newQuantity = inventory.Quantity + quantityChange;
            if (newQuantity < 0)
            {
                throw new InvalidOperationException(
                    $"Cannot adjust stock. Result would be negative. Current: {inventory.Quantity}, Change: {quantityChange}");
            }

            inventory.Quantity = newQuantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            UpdateInventoryStatus(inventory);

            await _context.SaveChangesAsync();
            return true;
        }

        private void UpdateInventoryStatus(Domain.Entities.Inventory inventory)
        {
            if (inventory.Quantity <= 0)
                inventory.Status = InventoryStatusEnum.OutOfStock;
            else if (inventory.Quantity <= inventory.MinimumStockLevel)
                inventory.Status = InventoryStatusEnum.LowStock;
            else
                inventory.Status = InventoryStatusEnum.Available;
        }

    }
}
