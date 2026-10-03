using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SalesInventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IValidator<CreateOrderDto> _validator;

        public OrdersController(IOrderService orderService, IValidator<CreateOrderDto> validator)
        {
            _orderService = orderService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAll() =>
            Ok(await _orderService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return order is null ? NotFound($"Order with id {id} not found") : Ok(order);
        }

        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var created = await _orderService.CreateAsync(dto); 
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}