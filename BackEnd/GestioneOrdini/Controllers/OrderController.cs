using AutoMapper;
using GestioneOrdini.Hubs;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using GestioneOrdini.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ILogger<OrderController> _logger;
    private readonly IMapper _mapper;

    public OrderController(IOrderService orderService,
        ICustomerService customerService,
        IHubContext<OrderHub> hubContext, 
        ILogger<OrderController> logger, 
        IMapper mapper)
    {
        _orderService = orderService;
        _customerService = customerService;
        _hubContext = hubContext;
        _logger = logger;
        _mapper = mapper;
    }

    //Crea un nuovo ordine
    [HttpPost("create")]
    [Authorize(Roles = "Admin, BackOffice, FrontOffice")]
    public async Task<IActionResult> CreateOrder([FromForm] OrderCreationDto orderCreationDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model validation failed for CreateOrder request.");
            return BadRequest(ModelState);
        }

        if (!int.TryParse(orderCreationDto.CustomerId.ToString(), out int customerId) || customerId <= 0)
        {
            _logger.LogWarning("Invalid CustomerId provided: {CustomerId}", orderCreationDto.CustomerId);
            return BadRequest("Invalid CustomerId. Must be a positive integer.");
        }

        try
        {
            // Mappatura del DTO all'entità Order usando AutoMapper
            var order = _mapper.Map<Order>(orderCreationDto);

            // Recupera il cliente dal database
            order.Customer = await _customerService.GetCustomerByIdAsync(orderCreationDto.CustomerId);

            if (order.Customer == null)
            {
                return NotFound("Customer not found."); // O un altro tipo di errore appropriato
            }

            // Creazione dell'ordine con gli item e i file associati
            var createdOrder = await _orderService.CreateOrderAsync(order, orderCreationDto.ItemsWithFiles);

            // Notifica tramite SignalR
            await _hubContext.Clients.All.SendAsync("OrderCreated", createdOrder);

            // Mappa l'ordine creato al DTO prima di restituirlo
            var createdOrderDto = _mapper.Map<OrderDto>(createdOrder);

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrderDto.Id }, createdOrderDto);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update exception while creating order: {Message}", ex.Message);
            if (ex.InnerException is SqlException sqlException && sqlException.Number == 547)
            {
                return BadRequest("Error creating order: Foreign key constraint violation. Check CustomerId.");
            }
            return StatusCode(500, "An unexpected error occurred while creating the order.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while creating order: {Message}", ex.Message);
            return StatusCode(500, "An unexpected error occurred while creating the order.");
        }
    }


    // Ottieni ordine per ID
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order != null)
        {
            // Mappatura con AutoMapper
            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }
        _logger.LogWarning("Order with ID {Id} not found.", id);
        return NotFound();
    }



    // Ottieni tutti gli ordini
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        // Mappatura con AutoMapper
        var ordersDto = _mapper.Map<List<OrderDto>>(orders);
        return Ok(ordersDto);
    }

    // Ottieni tutti gli stati degli ordini
    [HttpGet("statuses")]
    public async Task<IActionResult> GetAllOrderStatuses()
    {
        var statuses = await _orderService.GetAllOrderStatusesAsync();
        return Ok(statuses);
    }

    // Aggiornamento di un ordine
    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin, BackOffice, FrontOffice")]
    public async Task<IActionResult> UpdateOrder(int id, [FromForm] OrderWithItemsDto orderWithItemsDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model validation failed for UpdateOrder request.");
            return BadRequest(new
            {
                Title = "One or more validation errors occurred.",
                Status = 400,
                Errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
            });
        }

        var existingOrder = await _orderService.GetOrderByIdAsync(id);
        if (existingOrder == null)
        {
            _logger.LogWarning("Order with ID {Id} not found for update.", id);
            return NotFound();
        }

        // Mappatura di OrderWithItemsDto all'entità Order
        var updatedOrder = _mapper.Map<Order>(orderWithItemsDto.Order);

        // Aggiornamento ordine
        await _orderService.UpdateOrderAsync(updatedOrder, orderWithItemsDto.ItemsWithFiles);
        return NoContent();
    }



    // Elimina un ordine
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin, BackOffice, FrontOffice")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(id);
        if (existingOrder == null)
        {
            _logger.LogWarning("Order with ID {Id} not found for deletion.", id);
            return NotFound();
        }

        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }

    // Assegna un ordine a un operatore
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
            _logger.LogError(ex, "Error assigning order to operator: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    // Aggiorna lo stato di un ordine
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
            _logger.LogError(ex, "Error updating order status: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    // Ottieni tutti i tipi di lavoro
    [HttpGet("workType")]
    public async Task<IActionResult> GetAllWorkType()
    {
        var workTypes = await _orderService.GetAllWorkTypesAsync();
        return Ok(workTypes);
    }
}

