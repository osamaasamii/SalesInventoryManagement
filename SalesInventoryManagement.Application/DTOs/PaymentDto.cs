namespace SalesInventoryManagement.Application.DTOs
{
    public class CreatePaymentIntentDto
    {
        public int OrderId { get; set; }
    }

    public class PaymentIntentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}