using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Application.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Entity -> DTO
            config.NewConfig<Booking.Domain.Entities.Booking, BookingResponseDto>();
            config.NewConfig<Booking.Domain.Entities.Booking, BookingListDto>();

            // DTO -> Entity
            config.NewConfig<BookingCreateDto, Booking.Domain.Entities.Booking>()
                .Ignore(dest => dest.BookingId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            config.NewConfig<BookingUpdateDto, Booking.Domain.Entities.Booking>()
                .Ignore(dest => dest.BookingId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);
        }
    }
}
