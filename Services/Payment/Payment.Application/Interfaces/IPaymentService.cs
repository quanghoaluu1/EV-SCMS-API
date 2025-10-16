using GrpcContract;

namespace Payment.Application.Interfaces;

public interface IPaymentService
{
    Task<ResultDto<object>> CreatePaymentLink(Guid bookingId);
    Task<ResultDto<bool>> UpdatePaymentAsync(string orderCode);
    Task<ResultDto<bool>> ConfirmCashPayment(Guid bookingId);
}