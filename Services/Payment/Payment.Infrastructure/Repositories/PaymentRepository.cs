using Microsoft.EntityFrameworkCore;
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

    public async Task<PaymentTransaction?> GetByOrderCode(string orderCode)
    {
        return await _context.PaymentTransactions.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
    }
    public async Task<bool> UpdatePaymentAsync(PaymentTransaction paymentTransaction)
    {
        _context.PaymentTransactions.Update(paymentTransaction);
        await _context.SaveChangesAsync();
        return true;
    }
}