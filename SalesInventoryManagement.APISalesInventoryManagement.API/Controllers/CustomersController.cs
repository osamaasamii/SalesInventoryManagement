using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Interfaces;

namespace SalesInventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IValidator<CreateCustomerDto> _validator;

        public CustomersController(ICustomerService customerService, IValidator<CreateCustomerDto> validator)
        {
            _customerService = customerService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerDto>>> GetAll() =>
            Ok(await _customerService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            return customer is null ? NotFound($"Customer with id {id} not found") : Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var created = await _customerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
