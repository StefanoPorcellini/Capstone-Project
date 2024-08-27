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

        // Create customer based on type
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdCustomer = await _customerService.CreateCustomerAsync(
                        request.CustomerType,
                        request.Name,
                        request.Address,
                        request.Email,
                        request.Tel,
                        request.CF,
                        request.PartitaIVA,
                        request.RagioneSociale
                    );

                    return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
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
                try
                {
                    await _customerService.UpdateCustomerAsync(customer);
                    return NoContent();
                }
                catch (ArgumentException ex)
                {
                    return NotFound(ex.Message);
                }
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

    public class CreateCustomerRequest
    {
        public string CustomerType { get; set; } // "Private" or "Company"
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string Tel { get; set; }
        public string? CF { get; set; } // Required if Private
        public string? PartitaIVA { get; set; } // Required if Company
        public string? RagioneSociale { get; set; } // Required if Company
    }
}
