using Inventory.Application.DTOs;
using Inventory.Application.IServices;
using Inventory.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using static Inventory.Application.DTOs.InventoryDTOs;

namespace Inventory.Api.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
        {
            var inventories = await _inventoryService.GetAllAsync();
            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryDto>> GetById(Guid id)
        {
            var inventory = await _inventoryService.GetByIdAsync(id);
            if (inventory == null) return NotFound();
            return Ok(inventory);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryDto>> Create(CreateInventoryDto dto)
        {
            var created = await _inventoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryDto>> Update(Guid id, UpdateInventoryDto dto)
        {
            var updated = await _inventoryService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _inventoryService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetByCategory(InventoryCategoryEnum category)
        {
            var inventories = await _inventoryService.GetByCategoryAsync(category);
            return Ok(inventories);
        }
    }
}
