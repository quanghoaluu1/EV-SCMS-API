using Booking.Application.DTOs;
using Booking.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            return Ok(booking);
        }

        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var bookings = await _bookingService.GetByCustomerIdAsync(customerId);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookingUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _bookingService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = "Booking not found" });

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _bookingService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Booking not found or already deleted" });

            return NoContent();
        }
    }
}
