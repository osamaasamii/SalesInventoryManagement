using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Interfaces;

namespace SalesInventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService) => _paymentService = paymentService;

        [HttpPost("create-intent")]
        public async Task<ActionResult<PaymentIntentResponseDto>> CreateIntent(CreatePaymentIntentDto dto)
        {
            var result = await _paymentService.CreatePaymentIntentAsync(dto.OrderId);
            return Ok(result);
        }

        [HttpPost("confirm/{paymentIntentId}")]
        public async Task<ActionResult<string>> Confirm(string paymentIntentId)
        {
            var status = await _paymentService.ConfirmPaymentAsync(paymentIntentId);
            return Ok(new { status });
        }

        [HttpPost("confirm-test/{paymentIntentId}")]
        public async Task<ActionResult<string>> ConfirmWithTestCard(string paymentIntentId)
        {
            var status = await _paymentService.ConfirmWithTestCardAsync(paymentIntentId);
            return Ok(new { status });
        }
    }
}