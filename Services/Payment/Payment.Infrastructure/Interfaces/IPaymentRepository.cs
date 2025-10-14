using Payment.Domain.Entities;

namespace Payment.Infrastructure.Interfaces;

public interface IPaymentRepository
{
    Task<PaymentTransaction> AddAsync(PaymentTransaction paymentTransaction);
    Task<PaymentTransaction?> GetByOrderCode(string orderCode);
    Task<bool> UpdatePaymentAsync(PaymentTransaction paymentTransaction);
}