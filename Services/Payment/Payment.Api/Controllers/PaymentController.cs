using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.Application.Interfaces;
using Payment.Domain.Dtos;

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
    public async Task<IActionResult> CreateLinkPayment([FromQuery] Guid bookingId)
    {
        var result = await _paymentService.CreatePaymentLink(bookingId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);       
        }
        return Ok(result);
    }
    [HttpPut("update")]
    public async Task<IActionResult> UpdatePayment([FromQuery] string orderCode)
    {
        var result = await _paymentService.UpdatePaymentAsync(orderCode);
        if (!result.IsSuccess)
        {
            return BadRequest(result);       
        }
        return Ok(result);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> PayOsWebhook()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            var payload = JsonConvert.DeserializeObject<PayOsWebhookPayload>(rawBody);
            if (payload is { Success: true, Data.Code: "00" })
            {
                var orderCode = payload.Data.OrderCode;
                long amount = payload.Data.Amount;
                await _paymentService.UpdatePaymentAsync(orderCode.ToString());
            }
            return Ok(new { Message = "Success" });
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal server error");
        }
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
