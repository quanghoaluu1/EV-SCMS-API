using Payment.Domain.Entities;
using Payment.Infrastructure.Interfaces;

namespace Payment.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;
    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentTransaction> AddAsync(PaymentTransaction paymentTransaction)
    {
        _context.PaymentTransactions.Add(paymentTransaction);
        await _context.SaveChangesAsync();
        return paymentTransaction;
    }
}