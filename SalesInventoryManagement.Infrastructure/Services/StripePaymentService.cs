using Microsoft.Extensions.Configuration;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Exceptions;
using SalesInventoryManagement.Application.Interfaces;
using Stripe;

namespace SalesInventoryManagement.Infrastructure.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StripePaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
            if (order is null)
                throw new NotFoundException($"Order with id {orderId} not found");

            var totalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            // Stripe بياخد المبلغ بالـ "قرش" مش بالجنيه/الدولار الكامل
            var amountInCents = (long)(totalAmount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                AllowedPaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", order.Id.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            order.PaymentIntentId = paymentIntent.Id;
            order.PaymentStatus = paymentIntent.Status;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return new PaymentIntentResponseDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id,
                Amount = totalAmount
            };
        }

        public async Task<string> ConfirmPaymentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            if (paymentIntent.Status == "succeeded")
                await MarkOrderAsConfirmedAsync(paymentIntentId, paymentIntent.Status);

            return paymentIntent.Status;
        }

        // 👇 الميثود الجديدة - للتجربة في Test Mode بس
        public async Task<string> ConfirmWithTestCardAsync(string paymentIntentId)
        {
            var confirmOptions = new PaymentIntentConfirmOptions
            {
                PaymentMethod = "pm_card_visa" // كارت فيزا تجريبي جاهز من Stripe
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(paymentIntentId, confirmOptions);

            if (paymentIntent.Status == "succeeded")
                await MarkOrderAsConfirmedAsync(paymentIntentId, paymentIntent.Status);

            return paymentIntent.Status;
        }

        // 👇 ميثود مساعدة خاصة (private) - استخرجناها عشان متكررتش نفس الكود مرتين
        private async Task MarkOrderAsConfirmedAsync(string paymentIntentId, string paymentStatus)
        {
            var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();
            var order = orders.FirstOrDefault(o => o.PaymentIntentId == paymentIntentId);
            if (order is not null)
            {
                order.Status = Domain.Entities.OrderStatus.Confirmed;
                order.PaymentStatus = paymentStatus;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}