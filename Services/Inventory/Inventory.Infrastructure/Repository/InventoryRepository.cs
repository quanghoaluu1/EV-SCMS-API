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

        public async Task AddAsync(Domain.Entities.Inventory inventory)
        {
            await _context.inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.inventories.FindAsync(id);
            if (entity != null)
            {
                _context.inventories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Domain.Entities.Inventory>> GetAllAsync()
        {
            return await _context.inventories.ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Inventory>> GetByCategoryAsync(InventoryCategoryEnum category)
        {
            return await _context.inventories
                .Where(i => i.Category == category)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Inventory?> GetByIdAsync(Guid id)
        {
            return await _context.inventories.FindAsync(id);
        }

        public async Task UpdateAsync(Domain.Entities.Inventory inventory)
        {
            _context.inventories.Update(inventory);
            await _context.SaveChangesAsync();
        }
    }
}
