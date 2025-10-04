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
            config.NewConfig<Domain.Entities.Maintenance, MaintenanceResponseDto>();

            // DTO -> Entity
            config.NewConfig<CreateMaintenanceDto, Domain.Entities.Maintenance>()
                .Ignore(dest => dest.MaintenanceId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            config.NewConfig<UpdateMaintenanceDto, Domain.Entities.Maintenance>()
                .Ignore(dest => dest.MaintenanceId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);
        }
    }
}
