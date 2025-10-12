using Maintenance.Domain.Enums;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Maintenance.Application.DTOs.MaintenanceDTOs;

namespace Maintenance.Application.Mapping
{
    public class MappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Entity -> DTO
            // Entity -> ResponseDto
            config.NewConfig<Domain.Entities.Maintenance, MaintenanceResponseDto>()
                .Map(dest => dest.ServiceTypeName, src => GetServiceTypeName(src.ServiceType))
                .Map(dest => dest.StatusName, src => src.Status.ToString());

            // Entity -> ListDto 
            config.NewConfig<Domain.Entities.Maintenance, MaintenanceListDto>();

            // CreateDto -> Entity 
            config.NewConfig<CreateMaintenanceDto, Domain.Entities.Maintenance>()
                .Ignore(dest => dest.MaintenanceId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            // UpdateDto -> Entity
            // Use IgnoreNullValues to only update non-null fields
            config.NewConfig<UpdateMaintenanceDto, Domain.Entities.Maintenance>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.MaintenanceId)
                .Ignore(dest => dest.VehicleId)  // Cannot change vehicle
                .Ignore(dest => dest.CustomerId) // Cannot change customer
                .Ignore(dest => dest.BookingId)  // Cannot change booking
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);
        }

        // Helper method for service type display name
        private static string GetServiceTypeName(MaintenanceServiceTypeEnum serviceType)
        {
            return serviceType switch
            {
                MaintenanceServiceTypeEnum.PeriodicMaintenance => "Bảo dưỡng định kỳ",
                MaintenanceServiceTypeEnum.BatteryCheck => "Kiểm tra pin",
                MaintenanceServiceTypeEnum.BrakeService => "Bảo dưỡng phanh",
                MaintenanceServiceTypeEnum.TireReplacement => "Thay lốp",
                MaintenanceServiceTypeEnum.SuspensionRepair => "Sửa chữa hệ thống treo",
                MaintenanceServiceTypeEnum.ElectricalRepair => "Sửa chữa hệ thống điện",
                MaintenanceServiceTypeEnum.ChargingSystemRepair => "Sửa chữa hệ thống sạc",
                MaintenanceServiceTypeEnum.GeneralRepair => "Sửa chữa chung",
                MaintenanceServiceTypeEnum.Inspection => "Kiểm tra tổng quát",
                MaintenanceServiceTypeEnum.Emergency => "Khẩn cấp",
                _ => serviceType.ToString()
            };
        }
    }
}
