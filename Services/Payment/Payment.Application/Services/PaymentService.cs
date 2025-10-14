using GrpcContract;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Payment.Application.Interfaces;
using Payment.Domain.Entities;
using Payment.Domain.Enum;
using Payment.Infrastructure.Interfaces;

namespace Payment.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly PayOS _payos;
    private readonly Random random;
    private readonly IBookingGrpcGateway _bookingGrpcGateway;
    private readonly ICustomerGrpcGateway _customerGrpcGateway;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IConfiguration configuration, IBookingGrpcGateway bookingGrpcGateway, ICustomerGrpcGateway customerGrpcGateway, IPaymentRepository paymentRepository)
    {
        _configuration = configuration;
        _payos = new PayOS(
            _configuration["Payos:CLIENT_ID"] ?? throw new Exception("Cannot find PAYOS_CLIENT_ID"),
            _configuration["Payos:API_KEY"] ?? throw new Exception("Cannot find PAYOS_API_KEY"),
            _configuration["Payos:CHECKSUM_KEY"] ?? throw new Exception("Cannot find PAYOS_CHECKSUM_KEY")
            );
        random = new Random();
        _bookingGrpcGateway = bookingGrpcGateway;
        _customerGrpcGateway = customerGrpcGateway;
        _paymentRepository = paymentRepository;
    }

    public async Task<ResultDto<object>> CreatePaymentLink(Guid bookingId)
    {
        try
        {
            var bookingByIdReply = await _bookingGrpcGateway.GetBookingById(bookingId);
            if (!bookingByIdReply.Success)
            {
                return new ResultDto<object>()
                {
                    IsSuccess = false,
                    Message = bookingByIdReply.Message,
                };
            }

            var customer = await _customerGrpcGateway.GetCustomerById(Guid.Parse(bookingByIdReply.CustomerId));
            if (!customer.Success)
            {
                return new ResultDto<object>()
                {
                    IsSuccess = false,
                    Message = customer.Message,
                };
            }

            var items = new List<ItemData>
            {
                new ItemData(bookingByIdReply.ServiceType.ToString(), 1, Convert.ToInt32(bookingByIdReply.TotalCost))
            };
            var totalPrice = Convert.ToInt32(bookingByIdReply.TotalCost);
            var orderCode = Math.Abs(int.Parse(DateTimeOffset.Now.ToString("fffffff")) + customer.CustomerId.GetHashCode() + random.Next(1000, 9999));

            PaymentData paymentData = new PaymentData(
                orderCode: orderCode,
                amount: totalPrice,
                items: items,
                description: "EV-SCMS Booking Payment",
                returnUrl: "google.com",
                cancelUrl: ""
            );

            if (paymentData is null)
            {
                return new ResultDto<object>()
                {
                    IsSuccess = false,
                    Message = "Failed to create payment data",
                };
            }

            var response = await _payos.createPaymentLink(paymentData);
        
            PaymentTransaction paymentTransaction = new PaymentTransaction
            {
                BookingId = bookingId,
                CustomerId = Guid.Parse(customer.CustomerId),
                OrderCode = orderCode.ToString(),
                Amount = totalPrice,
                Method = PaymentMethod.BankTransfer,
                Status = PaymentStatus.Unpaid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _paymentRepository.AddAsync(paymentTransaction);

            return new ResultDto<object>()
            {
                IsSuccess = true,
                Message = "Payment link created successfully",
                Data = new
                {
                    PaymentLink = response,
                    PaymentTransaction = result.Id
                }
            };
        }
        catch (Exception e)
        {
            return new ResultDto<object>()
            {
                IsSuccess = false,
                Message = e.Message,
            };
        }

    }
}