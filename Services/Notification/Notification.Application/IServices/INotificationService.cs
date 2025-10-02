using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Notification.Application.DTOs.NotificationDTOs;

namespace Notification.Application.IServices
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponseDto>> GetAllAsync();
        Task<NotificationResponseDto?> GetByIdAsync(Guid id);
        Task<NotificationResponseDto> CreateAsync(CreateNotificationDto dto);
        Task<NotificationResponseDto?> UpdateAsync(Guid id, UpdateNotificationDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
