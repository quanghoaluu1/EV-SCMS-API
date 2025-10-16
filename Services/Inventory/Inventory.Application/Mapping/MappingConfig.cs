using Inventory.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;

namespace Inventory.Application.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
    {
        // Entity -> DTO
        config.NewConfig<Inventory.Domain.Entities.Inventory, InventoryDto>();

        // DTO -> Entity
        config.NewConfig<InventoryDTOs.CreateInventoryDto, Inventory.Domain.Entities.Inventory>()
            .Ignore(dest => dest.Id)          
            .Ignore(dest => dest.CreatedAt)  
            .Ignore(dest => dest.UpdatedAt); 

        config.NewConfig<UpdateInventoryDto, Inventory.Domain.Entities.Inventory>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt);
    }
}
}
