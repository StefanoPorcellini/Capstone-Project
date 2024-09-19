using GestioneOrdini.Hubs;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _hubContext;


    public OrderController(IOrderService orderService, IHubContext<OrderHub> hubContext)
    {
        _orderService = orderService;
        _hubContext = hubContext;
    }

    // Create an Order
    [HttpPost("create")]
    [Authorize(Roles = "Admin, BackOffice, FrontOffice")]

    public async Task<IActionResult> CreateOrder([FromForm] OrderWithFileDto orderWithFile)
    {
        if (ModelState.IsValid)
        {
            var createdOrder = await _orderService.CreateOrderAsync(orderWithFile.Order, orderWithFile.File);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }
        return BadRequest(ModelState);
    }


    // Get Order by ID
    [HttpGet("getById/{id}")]
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
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    // Get all Order Statuses
    [HttpGet("statuses")]
    public async Task<IActionResult> GetAllOrderStatuses()
    {
        var statuses = await _orderService.GetAllOrderStatusesAsync();
        return Ok(statuses);
    }

    // Update an Order
    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin, BackOffice, FrontOffice")]
    public async Task<IActionResult> UpdateOrder(int id, [FromForm] OrderWithFileDto orderWithFile)
    {
        if (id != orderWithFile.Order.Id)
        {
            return BadRequest("ID mismatch.");
        }

        if (ModelState.IsValid)
        {
            await _orderService.UpdateOrderAsync(orderWithFile.Order, orderWithFile.File);
            return NoContent();
        }
        return BadRequest(ModelState);
    }


    // Delete an Order
    [HttpDelete("delete/{id}")] // Corretto il typo "delate" in "delete"
    [Authorize(Roles = "Admin, BackOffice, FrontOffice")]

    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }

    // Assign Order to Operator
    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin, BackOffice")]

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
    [Authorize(Roles = "Admin, BackOffice, FrontOffice")]
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
