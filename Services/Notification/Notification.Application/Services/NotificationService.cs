using Mapster;
using Notification.Application.IServices;
using Notification.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Notification.Application.DTOs.NotificationDTOs;

namespace Notification.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetAllAsync()
        {
            var notifications = await _notificationRepository.GetAllAsync();
            return notifications.Adapt<IEnumerable<NotificationResponseDto>>();
        }

        public async Task<NotificationResponseDto?> GetByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            return notification?.Adapt<NotificationResponseDto>();
        }

        public async Task<NotificationResponseDto> CreateAsync(CreateNotificationDto dto)
        {
            var notification = dto.Adapt<Domain.Entities.Notification>();
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();
            return notification.Adapt<NotificationResponseDto>();
        }

        public async Task<NotificationResponseDto?> UpdateAsync(Guid id, UpdateNotificationDto dto)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return null;

            dto.Adapt(notification); // Mapster sẽ map field từ dto sang entity
            await _notificationRepository.UpdateAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            return notification.Adapt<NotificationResponseDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return false;

            await _notificationRepository.DeleteAsync(id);
            await _notificationRepository.SaveChangesAsync();
            return true;
        }
    }
}
