using Inventory.Application.DTOs;
using Inventory.Domain.Enums;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Inventory.Application.DTOs.InventoryDTOs;

namespace Inventory.Application.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.Entities.Inventory, InventoryResponseDto>()
                .Map(dest => dest.CategoryName, src => GetCategoryName(src.Category))
                .Map(dest => dest.StatusName, src => GetStatusName(src.Status));

            config.NewConfig<Domain.Entities.Inventory, InventoryListDto>();

            config.NewConfig<CreateInventoryDto, Domain.Entities.Inventory>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Status) 
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            config.NewConfig<UpdateInventoryDto, Domain.Entities.Inventory>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);
        }

        private static string GetCategoryName(InventoryCategoryEnum category)
        {
            return category switch
            {
                InventoryCategoryEnum.Battery => "Pin",
                InventoryCategoryEnum.BrakeParts => "Phụ tùng phanh",
                InventoryCategoryEnum.Tires => "Lốp xe",
                InventoryCategoryEnum.SuspensionParts => "Hệ thống treo",
                InventoryCategoryEnum.ElectricalParts => "Linh kiện điện",
                InventoryCategoryEnum.ChargingComponents => "Linh kiện sạc",
                InventoryCategoryEnum.BodyParts => "Phụ tùng thân xe",
                InventoryCategoryEnum.InteriorParts => "Nội thất",
                InventoryCategoryEnum.Fluids => "Dầu/Nước",
                InventoryCategoryEnum.Tools => "Dụng cụ",
                InventoryCategoryEnum.Accessories => "Phụ kiện",
                _ => category.ToString()
            };
        }

        private static string GetStatusName(InventoryStatusEnum status)
        {
            return status switch
            {
                InventoryStatusEnum.Available => "Còn hàng",
                InventoryStatusEnum.LowStock => "Sắp hết",
                InventoryStatusEnum.OutOfStock => "Hết hàng",
                InventoryStatusEnum.Discontinued => "Ngừng kinh doanh",
                _ => status.ToString()
            };
        }
    }
}
