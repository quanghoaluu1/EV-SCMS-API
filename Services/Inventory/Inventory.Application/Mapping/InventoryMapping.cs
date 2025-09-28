using Inventory.Application.DTOs;
using Inventory.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Mapping
{
    public class InventoryMapping
    {
        public static void Configure()
        {
            // Entity → DTO
            TypeAdapterConfig<Domain.Entities.Inventory, InventoryDto>
                .NewConfig()
                .Map(dest => dest.status, src => src.status); // nếu tên property khác nhau

            // CreateDto → Entity
            TypeAdapterConfig<CreateInventoryDto, Domain.Entities.Inventory>
                .NewConfig()
                .Map(dest => dest.Id, src => Guid.NewGuid())       // tạo Id mới
                .Map(dest => dest.status, src => "Available")     // mặc định status
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);

            // UpdateDto → Entity
            TypeAdapterConfig<UpdateInventoryDto, Domain.Entities.Inventory>
                .NewConfig()
                .Ignore(dest => dest.Id)           // không được update Id
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
        }
    }
}
