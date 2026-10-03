using SalesInventoryManagement.Application.DTOs;

namespace SalesInventoryManagement.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(int orderId);
        Task<string> ConfirmPaymentAsync(string paymentIntentId);
        Task<string> ConfirmWithTestCardAsync(string paymentIntentId);
    }
}