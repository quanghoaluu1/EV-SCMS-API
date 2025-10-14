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
        return Ok(result);
    }
}