using Microsoft.AspNetCore.Mvc;
using Notification.Application.IServices;
using static Notification.Application.DTOs.NotificationDTOs;

namespace Notification.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/notification
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationService.GetAllAsync();
            return Ok(notifications);
        }

        // GET: api/notification/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _notificationService.GetByIdAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }

        // POST: api/notification
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _notificationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.NotificationId }, created);
        }

        // PUT: api/notification/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotificationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _notificationService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE: api/notification/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _notificationService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
