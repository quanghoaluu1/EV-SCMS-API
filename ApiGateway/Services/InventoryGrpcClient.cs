using InventoryService.Grpc;

namespace ApiGateway.Services
{
    public interface IInventoryGrpcClient
    {
        Task<GetInventoryItemResponse> GetInventoryItemAsync(Guid id);
        Task<GetAllInventoryItemsResponse> GetAllInventoryItemsAsync();
        Task<CreateInventoryItemResponse> CreateInventoryItemAsync(string name, int category, int quantity, decimal unitPrice, string status);
        Task<UpdateInventoryItemResponse> UpdateInventoryItemAsync(Guid id, string name, int category, int quantity, decimal unitPrice, string status);
        Task<DeleteInventoryItemResponse> DeleteInventoryItemAsync(Guid id);
        Task<GetByCategoryResponse> GetByCategoryAsync(int category);
    }

    public class InventoryGrpcClient : IInventoryGrpcClient
    {
        private readonly InventoryService.Grpc.InventoryService.InventoryServiceClient _client;
        private readonly ILogger<InventoryGrpcClient> _logger;

        public InventoryGrpcClient(InventoryService.Grpc.InventoryService.InventoryServiceClient client, ILogger<InventoryGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GetInventoryItemResponse> GetInventoryItemAsync(Guid id)
        {
            try
            {
                var request = new GetInventoryItemRequest { Id = id.ToString() };
                return await _client.GetInventoryItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory Service - GetInventoryItem for ID: {Id}", id);
                throw;
            }
        }

        public async Task<GetAllInventoryItemsResponse> GetAllInventoryItemsAsync()
        {
            try
            {
                var request = new GetAllInventoryItemsRequest();
                return await _client.GetAllInventoryItemsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory Service - GetAllInventoryItems");
                throw;
            }
        }

        public async Task<CreateInventoryItemResponse> CreateInventoryItemAsync(string name, int category, int quantity, decimal unitPrice, string status)
        {
            try
            {
                var request = new CreateInventoryItemRequest
                {
                    Name = name,
                    Category = category,
                    Quantity = quantity,
                    UnitPrice = (double)unitPrice,
                    Status = status
                };
                return await _client.CreateInventoryItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory Service - CreateInventoryItem");
                throw;
            }
        }

        public async Task<UpdateInventoryItemResponse> UpdateInventoryItemAsync(Guid id, string name, int category, int quantity, decimal unitPrice, string status)
        {
            try
            {
                var request = new UpdateInventoryItemRequest
                {
                    Id = id.ToString(),
                    Name = name,
                    Category = category,
                    Quantity = quantity,
                    UnitPrice = (double)unitPrice,
                    Status = status
                };
                return await _client.UpdateInventoryItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory Service - UpdateInventoryItem for ID: {Id}", id);
                throw;
            }
        }

        public async Task<DeleteInventoryItemResponse> DeleteInventoryItemAsync(Guid id)
        {
            try
            {
                var request = new DeleteInventoryItemRequest { Id = id.ToString() };
                return await _client.DeleteInventoryItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory Service - DeleteInventoryItem for ID: {Id}", id);
                throw;
            }
        }

        public async Task<GetByCategoryResponse> GetByCategoryAsync(int category)
        {
            try
            {
                var request = new GetByCategoryRequest { Category = category };
                return await _client.GetByCategoryAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory Service - GetByCategory for Category: {Category}", category);
                throw;
            }
        }
    }
}