using Booking.Application.IServices;
using Booking.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/bookings?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingListDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _bookingService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // GET: api/bookings/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookingResponseDto>> GetById(Guid id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create([FromBody] BookingCreateDto dto)
        {
            var created = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, created);
        }

        // PUT: api/bookings/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BookingResponseDto>> Update(Guid id, [FromBody] BookingUpdateDto dto)
        {
            var updated = await _bookingService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _bookingService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // PATCH: api/bookings/{id}/status
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] BookingStatusEnum newStatus)
        {
            var success = await _bookingService.UpdateStatusAsync(id, newStatus);
            if (!success) return NotFound();
            return NoContent();
        }

        // PATCH: api/bookings/{id}/assign/{staffId}
        [HttpPatch("{id:guid}/assign/{staffId:guid}")]
        public async Task<IActionResult> AssignStaff(Guid id, Guid staffId)
        {
            var success = await _bookingService.AssignStaffAsync(id, staffId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
