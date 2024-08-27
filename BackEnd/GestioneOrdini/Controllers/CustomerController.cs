using GestioneOrdini.Interface;
using GestioneOrdini.Model.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Create customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var createdCustomer = await _customerService.CreateCustomerAsync(customer);
                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("company")]
        public async Task<IActionResult> CreateCustomerCompany([FromBody] CustomerCompany customerCompany)
        {
            if (ModelState.IsValid)
            {
                var createdCustomerCompany = await _customerService.CreateCustomerCompanyAsync(customerCompany);
                return CreatedAtAction(nameof(GetCustomerCompanyById), new { id = createdCustomerCompany.Id }, createdCustomerCompany);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("private")]
        public async Task<IActionResult> CreateCustomerPrivate([FromBody] CustomerPrivate customerPrivate)
        {
            if (ModelState.IsValid)
            {
                var createdCustomerPrivate = await _customerService.CreateCustomerPrivateAsync(customerPrivate);
                return CreatedAtAction(nameof(GetCustomerPrivateById), new { id = createdCustomerPrivate.Id }, createdCustomerPrivate);
            }
            return BadRequest(ModelState);
        }

        // Get all customers
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        // Get customer by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer != null)
            {
                return Ok(customer);
            }
            return NotFound();
        }

        [HttpGet("company/{id}")]
        public async Task<IActionResult> GetCustomerCompanyById(int id)
        {
            var customerCompany = await _customerService.GetCustomerCompanyByIdAsync(id);
            if (customerCompany != null)
            {
                return Ok(customerCompany);
            }
            return NotFound();
        }

        [HttpGet("private/{id}")]
        public async Task<IActionResult> GetCustomerPrivateById(int id)
        {
            var customerPrivate = await _customerService.GetCustomerPrivateByIdAsync(id);
            if (customerPrivate != null)
            {
                return Ok(customerPrivate);
            }
            return NotFound();
        }

        // Update customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest("ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                await _customerService.UpdateCustomerAsync(customer);
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // Delete customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
