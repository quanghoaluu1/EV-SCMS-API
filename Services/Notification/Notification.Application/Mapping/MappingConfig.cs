using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Notification.Application.DTOs.NotificationDTOs;

namespace Notification.Application.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Entity -> DTO
            config.NewConfig<Domain.Entities.Notification, NotificationResponseDto>();

            // DTO -> Entity
            config.NewConfig<CreateNotificationDto, Domain.Entities.Notification>()
                .Ignore(dest => dest.NotificationId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.ReadAt);

            config.NewConfig<UpdateNotificationDto, Domain.Entities.Notification>()
                .Ignore(dest => dest.NotificationId)
                .Ignore(dest => dest.CustomerId)
                .Ignore(dest => dest.VehicleId)
                .Ignore(dest => dest.CreatedAt);
        }
    }
}
