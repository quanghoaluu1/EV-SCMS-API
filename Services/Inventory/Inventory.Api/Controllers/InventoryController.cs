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

        [HttpPost]
        public async Task<ActionResult<InventoryResponseDto>> CreateInventory([FromBody] CreateInventoryDto dto)
        {
            try
            {
                var result = await _inventoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetInventoryById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<InventoryResponseDto>> GetInventoryById(Guid id)
        {
            var result = await _inventoryService.GetByIdAsync(id);
            if (result == null)
                return NotFound($"Inventory item with ID {id} not found");

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryListDto>>> GetAllInventories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _inventoryService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<InventoryResponseDto>> UpdateInventory(Guid id, [FromBody] UpdateInventoryDto dto)
        {
            try
            {
                var result = await _inventoryService.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound($"Inventory item with ID {id} not found");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteInventory(Guid id)
        {
            var result = await _inventoryService.DeleteAsync(id);
            if (!result)
                return NotFound($"Inventory item with ID {id} not found");

            return NoContent();
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<InventoryListDto>>> GetInventoryByCategory(InventoryCategoryEnum category)
        {
            var result = await _inventoryService.GetByCategoryAsync(category);
            return Ok(result);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<InventoryListDto>>> GetInventoryByStatus(InventoryStatusEnum status)
        {
            var result = await _inventoryService.GetByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("alerts")]
        public async Task<ActionResult<InventoryAlertDto>> GetAlerts()
        {
            var result = await _inventoryService.GetAlertsAsync();
            return Ok(result);
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<InventoryListDto>>> GetLowStockItems()
        {
            var result = await _inventoryService.GetLowStockItemsAsync();
            return Ok(result);
        }

        [HttpGet("out-of-stock")]
        public async Task<ActionResult<IEnumerable<InventoryListDto>>> GetOutOfStockItems()
        {
            var result = await _inventoryService.GetOutOfStockItemsAsync();
            return Ok(result);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<InventorySummaryDto>> GetSummary()
        {
            var result = await _inventoryService.GetSummaryAsync();
            return Ok(result);
        }

        [HttpPost("{id:guid}/adjust-stock")]
        public async Task<ActionResult> AdjustStock(Guid id, [FromBody] AdjustStockDto dto)
        {
            try
            {
                var result = await _inventoryService.AdjustStockAsync(id, dto);
                if (!result)
                    return NotFound($"Inventory item with ID {id} not found");

                return Ok(new { message = "Stock adjusted successfully", quantityChange = dto.QuantityChange });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
