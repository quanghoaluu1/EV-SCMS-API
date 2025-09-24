using Booking.Application.DTOs;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace Booking.Application.Mapping
{
    public class BookingMapping
    {
        public static void Configure()
        {
            // Entity → DTO
            TypeAdapterConfig<Domain.Entities.Booking, BookingDTOs>
                .NewConfig();

            // CreateDto → Entity
            TypeAdapterConfig<BookingCreateDto, Domain.Entities.Booking>
                .NewConfig()
                .Map(dest => dest.BookingId, src => Guid.NewGuid())
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.Status, src => BookingStatusEnum.Pending);

            // UpdateDto → Entity
            TypeAdapterConfig<BookingUpdateDto, Domain.Entities.Booking>
                .NewConfig()
                .Ignore(dest => dest.BookingId)
                .Ignore(dest => dest.CustomerId)
                .Ignore(dest => dest.VehicleId)
                .Ignore(dest => dest.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
        }
    }
}
