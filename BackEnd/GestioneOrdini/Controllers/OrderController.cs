using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, BackEnd")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Create an Order
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                var createdOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
            }
            return BadRequest(ModelState);
        }

        // Get Order by ID
        [HttpGet("getById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order != null)
            {
                return Ok(order);
            }
            return NotFound();
        }

        // Get all Orders
        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // Get all Order Statuses
        [HttpGet("statuses")]
        [Authorize]
        public async Task<IActionResult> GetAllOrderStatuses()
        {
            var statuses = await _orderService.GetAllOrderStatusesAsync();
            return Ok(statuses);
        }

        // Update an Order
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id)
            {
                return BadRequest("ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                await _orderService.UpdateOrderAsync(order);
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // Delete an Order
        [HttpDelete("delate/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }

        // Assign Order to Operator
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignOrderToOperator(int id)
        {
            try
            {
                await _orderService.AssignOrderToOperatorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Update Order Status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin, BackEnd, FrontEnd")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] int newStatusId)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, newStatusId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
