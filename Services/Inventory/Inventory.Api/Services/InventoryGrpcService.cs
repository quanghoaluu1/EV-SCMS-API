using Grpc.Core;
using Inventory.Application.DTOs;
using Inventory.Application.IServices;
using Inventory.Domain.Enums;
using InventoryService.Grpc;
using Mapster;

namespace Inventory.Api.Services
{
    public class InventoryGrpcService : InventoryService.Grpc.InventoryService.InventoryServiceBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryGrpcService> _logger;

        public InventoryGrpcService(
            IInventoryService inventoryService,
            ILogger<InventoryGrpcService> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public override async Task<GetInventoryItemResponse> GetInventoryItem(
            GetInventoryItemRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id))
                {
                    return new GetInventoryItemResponse
                    {
                        Success = false,
                        Message = "Invalid ID format"
                    };
                }

                var item = await _inventoryService.GetByIdAsync(id);

                if (item == null)
                {
                    return new GetInventoryItemResponse
                    {
                        Success = false,
                        Message = $"Inventory item with ID {request.Id} not found"
                    };
                }

                return new GetInventoryItemResponse
                {
                    Item = MapToGrpcInventoryItem(item),
                    Success = true,
                    Message = "Item retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory item with ID: {Id}", request.Id);
                return new GetInventoryItemResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<CreateInventoryItemResponse> CreateInventoryItem(
            CreateInventoryItemRequest request,
            ServerCallContext context)
        {
            try
            {
                var createDto = new CreateInventoryDto
                {
                    Name = request.Name,
                    Category = (InventoryCategoryEnum)request.Category,
                    Quantity = request.Quantity,
                    UnitPrice = (decimal)request.UnitPrice,
                    status = request.Status
                };

                var createdItem = await _inventoryService.CreateAsync(createDto);

                return new CreateInventoryItemResponse
                {
                    Item = MapToGrpcInventoryItem(createdItem),
                    Success = true,
                    Message = "Item created successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item");
                return new CreateInventoryItemResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<UpdateInventoryItemResponse> UpdateInventoryItem(
            UpdateInventoryItemRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id))
                {
                    return new UpdateInventoryItemResponse
                    {
                        Success = false,
                        Message = "Invalid ID format"
                    };
                }

                var updateDto = new UpdateInventoryDto
                {
                    Name = request.Name,
                    Category = (InventoryCategoryEnum)request.Category,
                    Quantity = request.Quantity,
                    UnitPrice = (decimal)request.UnitPrice,
                    status = request.Status
                };

                var updatedItem = await _inventoryService.UpdateAsync(id, updateDto);

                if (updatedItem == null)
                {
                    return new UpdateInventoryItemResponse
                    {
                        Success = false,
                        Message = $"Inventory item with ID {request.Id} not found"
                    };
                }

                return new UpdateInventoryItemResponse
                {
                    Item = MapToGrpcInventoryItem(updatedItem),
                    Success = true,
                    Message = "Item updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory item with ID: {Id}", request.Id);
                return new UpdateInventoryItemResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<DeleteInventoryItemResponse> DeleteInventoryItem(
            DeleteInventoryItemRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id))
                {
                    return new DeleteInventoryItemResponse
                    {
                        Success = false,
                        Message = "Invalid ID format"
                    };
                }

                var success = await _inventoryService.DeleteAsync(id);

                return new DeleteInventoryItemResponse
                {
                    Success = success,
                    Message = success ? "Item deleted successfully" : $"Item with ID {request.Id} not found"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory item with ID: {Id}", request.Id);
                return new DeleteInventoryItemResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<GetAllInventoryItemsResponse> GetAllInventoryItems(
            GetAllInventoryItemsRequest request,
            ServerCallContext context)
        {
            try
            {
                var items = await _inventoryService.GetAllAsync();

                var response = new GetAllInventoryItemsResponse
                {
                    Success = true,
                    Message = "Items retrieved successfully"
                };

                response.Items.AddRange(items.Select(MapToGrpcInventoryItem));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all inventory items");
                return new GetAllInventoryItemsResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<GetByCategoryResponse> GetByCategory(
            GetByCategoryRequest request,
            ServerCallContext context)
        {
            try
            {
                var category = (InventoryCategoryEnum)request.Category;
                var items = await _inventoryService.GetByCategoryAsync(category);

                var response = new GetByCategoryResponse
                {
                    Success = true,
                    Message = "Items retrieved successfully"
                };

                response.Items.AddRange(items.Select(MapToGrpcInventoryItem));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory items by category: {Category}", request.Category);
                return new GetByCategoryResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        private static InventoryService.Grpc.InventoryItem MapToGrpcInventoryItem(InventoryDto item)
        {
            return new InventoryService.Grpc.InventoryItem
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                Category = (int)item.Category,
                Quantity = item.Quantity,
                UnitPrice = (double)item.UnitPrice,
                Status = item.status,
                CreatedAt = item.CreatedAt.Ticks,
                UpdatedAt = item.UpdatedAt?.Ticks ?? 0
            };
        }
    }

}
