using GestioneOrdini.Hubs;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Clients;
using GestioneOrdini.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,FrontOffice")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IHubContext<OrderHub> _hubContext;

        public CustomerController(ICustomerService customerService, IHubContext<OrderHub> hubContext)
        {
            _customerService = customerService;
            _hubContext = hubContext;
        }

        // Create customer based on type
        [HttpPost("create")]
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

                    // Invia un messaggio a tutti i client connessi tramite SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveCustomerUpdate", "Un nuovo cliente è stato creato!");

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
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        // Get customer by ID
        [HttpGet("getById/{id}")]
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
        [HttpPut("update/{id}")]
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

                    // Invia un messaggio a tutti i client connessi tramite SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveCustomerUpdate", $"Il cliente con ID {id} è stato aggiornato.");

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
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customerService.DeleteCustomerAsync(id);

            // Invia un messaggio a tutti i client connessi tramite SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveCustomerUpdate", $"Il cliente con ID {id} è stato eliminato.");

            return NoContent();
        }
    }


}
