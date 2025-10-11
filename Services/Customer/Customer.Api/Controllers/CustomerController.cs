using Customer.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using static Customer.Application.DTOs.CustomerDTOs;

namespace Customer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // POST: api/customer
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
        {
            try
            {
                var result = await _customerService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: api/customer/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { Message = "Customer not found" });

            return Ok(result);
        }

        // GET: api/customer/user/{userId}
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _customerService.GetByUserIdAsync(userId);
            if (result == null)
                return NotFound(new { Message = "Customer not found for this user" });

            return Ok(result);
        }

        // GET: api/customer
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { Message = "PageNumber and PageSize must be greater than 0" });

            var result = await _customerService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // GET: api/customer/status/{status}
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { Message = "PageNumber and PageSize must be greater than 0" });

            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { Message = "Status cannot be empty" });

            var result = await _customerService.GetByStatusAsync(status, pageNumber, pageSize);
            return Ok(result);
        }

        // PUT: api/customer/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerUpdateDto dto)
        {
            try
            {
                var result = await _customerService.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound(new { Message = "Customer not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PATCH: api/customer/{id}/status
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest(new { Message = "Status cannot be empty" });

            try
            {
                var success = await _customerService.UpdateStatusAsync(id, request.Status);
                if (!success)
                    return NotFound(new { Message = "Customer not found" });

                return Ok(new { Message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/customer/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _customerService.DeleteAsync(id);
                if (!success)
                    return NotFound(new { Message = "Customer not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    // Request model for status update
    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
