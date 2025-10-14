using System.Text.Json;
using System.Text.Json.Nodes;
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
            var totalPrice = bookingByIdReply.TotalCost;
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
                DataJson = JsonSerializer.Serialize(items),
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
    
    public async Task<ResultDto<bool>> UpdatePaymentAsync(string orderCode)
    {
        try
        {
            var payment = await _paymentRepository.GetByOrderCode(orderCode);
            if (payment is null)
            {
                return new ResultDto<bool>()
                {
                    IsSuccess = false,
                    Message = "Payment not found",
                    Data = false
                };
            }

            var paymentStatus = await _payos.getPaymentLinkInformation(long.Parse(orderCode));
            payment.Status = paymentStatus.status switch
            {
                "PAID" => PaymentStatus.Paid,
                "UNPAID" => PaymentStatus.Unpaid,
                "CANCELLED" => PaymentStatus.Cancelled,
                _ => payment.Status
            };

            var updateResult = await _paymentRepository.UpdatePaymentAsync(payment);
            if (!updateResult)
            {
                return new ResultDto<bool>()
                {
                    IsSuccess = false,
                    Message = "Failed to update payment",
                    Data = false
                };
            }

            return new ResultDto<bool>()
            {
                IsSuccess = true,
                Message = "Update payment successfully",
                Data = true
            };
        }
        catch (Exception e)
        {
            return new ResultDto<bool>()
            {
                IsSuccess = false,
                Message = e.Message,
                Data = false
            };
        }
    }

    public async Task<ResultDto<bool>> ConfirmCashPayment(Guid bookingId)
    {
        try
        {
            var bookingByIdReply = await _bookingGrpcGateway.GetBookingById(bookingId);
            if (!bookingByIdReply.Success)
            {
                return new ResultDto<bool>()
                {
                    IsSuccess = false,
                    Message = bookingByIdReply.Message,
                    Data = false
                };
            }

            var customer = await _customerGrpcGateway.GetCustomerById(Guid.Parse(bookingByIdReply.CustomerId));
            if (!customer.Success)
            {
                return new ResultDto<bool>()
                {
                    IsSuccess = false,
                    Message = customer.Message,
                    Data = false
                };
            }

            var orderCode = Math.Abs(int.Parse(DateTimeOffset.Now.ToString("fffffff")) +
                                     customer.CustomerId.GetHashCode() + random.Next(1000, 9999));
            var totalPrice = bookingByIdReply.TotalCost;

            var payment = new PaymentTransaction()
            {
                BookingId = bookingId,
                CustomerId = Guid.Parse(customer.CustomerId),
                OrderCode = orderCode.ToString(),
                Amount = totalPrice,
                Method = PaymentMethod.Cash,
                Status = PaymentStatus.Paid,
                ConfirmedBy = Guid.Parse(bookingByIdReply.StaffId),
                ConfirmedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);
            return new ResultDto<bool>()
            {
                IsSuccess = true,
                Message = "Payment by cash confirmed successfully",
                Data = true
            };
        }
        catch (Exception e)
        {
            return new ResultDto<bool>()
            {
                IsSuccess = false,
                Message = e.Message,
                Data = false
            };
        }
    }
}