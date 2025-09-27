using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Vehicle.Application.DTOs;
using Vehicle.Domain.Entities;

namespace Vehicle.Application.Mapping
{
    public class VehicleMapping
    {
        public static void Configure()
        {
            // Entity → DTO
            TypeAdapterConfig<Domain.Entities.Vehicle, VehicleDto>
                .NewConfig();

            // CreateDto → Entity
            TypeAdapterConfig<VehicleCreateDto, Domain.Entities.Vehicle>
                .NewConfig()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);

            // UpdateDto → Entity
            TypeAdapterConfig<VehicleUpdateDto, Domain.Entities.Vehicle>
                .NewConfig()
                .Ignore(dest => dest.Id)           // không được update Id
                .Ignore(dest => dest.CustomerId)   // không đổi chủ xe
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
        }
    }
}
