using Microsoft.AspNetCore.Mvc;
using Payment.Application.Interfaces;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateLinkPayment([FromBody] Guid bookingId)
    {
        var result = await _paymentService.CreatePaymentLink(bookingId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);       
        }
        return Ok(result);
    }
    [HttpPut("update")]
    public async Task<IActionResult> UpdatePayment([FromBody] string orderCode)
    {
        var result = await _paymentService.UpdatePaymentAsync(orderCode);
        if (!result.IsSuccess)
        {
            return BadRequest(result);       
        }
        return Ok(result);
    }

    [HttpPost("confirm-cash")]
    public async Task<IActionResult> ConfirmCashPayment(Guid bookingId)
    {
        var result = await _paymentService.ConfirmCashPayment(bookingId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);       
        }
        return Ok(result);
    }
}
