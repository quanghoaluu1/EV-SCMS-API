using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryGrpcClient _inventoryClient;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryGrpcClient inventoryClient, ILogger<InventoryController> logger)
        {
            _inventoryClient = inventoryClient;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItem(Guid id)
        {
            try
            {
                var response = await _inventoryClient.GetInventoryItemAsync(id);

                if (!response.Success)
                    return NotFound(response.Message);

                return Ok(response.Item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory item {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventoryItems()
        {
            try
            {
                var response = await _inventoryClient.GetAllInventoryItemsAsync();

                if (!response.Success)
                    return BadRequest(response.Message);

                return Ok(response.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all inventory items");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryRequest request)
        {
            try
            {
                var response = await _inventoryClient.CreateInventoryItemAsync(
                    request.Name, request.Category, request.Quantity, request.UnitPrice, request.Status);

                if (!response.Success)
                    return BadRequest(response.Message);

                return CreatedAtAction(nameof(GetInventoryItem), new { id = response.Item.Id }, response.Item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryItem(Guid id, [FromBody] UpdateInventoryRequest request)
        {
            try
            {
                var response = await _inventoryClient.UpdateInventoryItemAsync(
                    id, request.Name, request.Category, request.Quantity, request.UnitPrice, request.Status);

                if (!response.Success)
                    return NotFound(response.Message);

                return Ok(response.Item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory item {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(Guid id)
        {
            try
            {
                var response = await _inventoryClient.DeleteInventoryItemAsync(id);

                if (!response.Success)
                    return NotFound(response.Message);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory item {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(int category)
        {
            try
            {
                var response = await _inventoryClient.GetByCategoryAsync(category);

                if (!response.Success)
                    return BadRequest(response.Message);

                return Ok(response.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory items by category {Category}", category);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    // Request DTOs for Gateway API
    public class CreateInventoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Category { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; } = "Available";
    }

    public class UpdateInventoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Category { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; } = "Available";
    }
}