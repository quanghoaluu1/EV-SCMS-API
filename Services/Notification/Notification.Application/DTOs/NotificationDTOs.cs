using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.DTOs
{
    public static class NotificationDTOs
    {
        public class NotificationResponseDto
        {
            public Guid NotificationId { get; set; }
            public Guid? CustomerId { get; set; }
            public Guid? VehicleId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? ReadAt { get; set; }
        }

        public class CreateNotificationDto
        {
            public Guid? CustomerId { get; set; }
            public Guid? VehicleId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        public class UpdateNotificationDto
        {
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public DateTime? ReadAt { get; set; }
        }
    }
}
