using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Customer.Application.DTOs.CustomerDTOs;

namespace Customer.Application.Mapping
{
    public class CustomerMapping
    {
        public static void Configure()
        {
            // Entity → ResponseDto
            TypeAdapterConfig<Domain.Entities.Customer, CustomerResponseDto>
                .NewConfig();

            // Entity → ListDto
            TypeAdapterConfig<Domain.Entities.Customer, CustomerListDto>
                .NewConfig();

            // CreateDto → Entity
            TypeAdapterConfig<CustomerCreateDto, Domain.Entities.Customer>
                .NewConfig()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.Status, src => "Active"); // Set default status

            // UpdateDto → Entity
            TypeAdapterConfig<CustomerUpdateDto, Domain.Entities.Customer>
                .NewConfig()
                .Ignore(dest => dest.Id)           // không được update Id
                .Ignore(dest => dest.UserId)       // không đổi UserId
                .Ignore(dest => dest.CreatedAt)    // không đổi CreatedAt
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
        }
    }
}
