using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class OrderService : IOrderService
{
    private readonly OrdersDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrderService(OrdersDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.StatusId = 1;
        order.TotalAmount = await CalculateTotalAmountAsync(order);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Item)
            .Include(o => o.Status)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Item)
            .Include(o => o.Status)
            .ToListAsync();
    }

    public async Task UpdateOrderAsync(Order order)
    {
        order.TotalAmount = await CalculateTotalAmountAsync(order);
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> CalculateTotalAmountAsync(Order order)
    {
        if (order.Item is LaserItem laserItem)
        {
            if (laserItem.IsCustom)
            {
                var priceList = await _context.LaserPriceLists
                    .Where(lp => lp.MinQuantity <= laserItem.Quantity &&
                                 (lp.MaxQuantity == null || laserItem.Quantity <= lp.MaxQuantity))
                    .FirstOrDefaultAsync();

                if (priceList != null)
                {
                    return priceList.UnitPrice * laserItem.Quantity.Value;
                }
                else
                {
                    throw new Exception("Nessun listino prezzi trovato per la quantità specificata.");
                }
            }
            else if (laserItem.LaserStandardId.HasValue)
            {
                var laserStandard = await _context.LaserStandards.FindAsync(laserItem.LaserStandardId.Value);
                if (laserStandard != null)
                {
                    return laserStandard.Price * laserItem.Quantity.Value;
                }
            }
        }
        else if (order.Item is PlotterItem plotterItem)
        {
            if (plotterItem.IsCustom)
            {
                var area = (decimal)(plotterItem.Base * plotterItem.Height);
                return area * (decimal)plotterItem.PricePerSquareMeter * plotterItem.Quantity.Value;
            }
            else if (plotterItem.PlotterStandardId.HasValue)
            {
                var plotterStandard = await _context.PlotterStandards.FindAsync(plotterItem.PlotterStandardId.Value);
                return plotterStandard.Price * plotterItem.Quantity.Value;
            }
        }

        return 0;
    }

    public async Task AssignOrderToOperatorAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            var operatorName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(operatorName))
            {
                throw new Exception("Non è stato possibile determinare l'utente loggato.");
            }

            order.OperatorName = operatorName;
            order.StatusId = 2;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateOrderStatusAsync(int orderId, int newStatusId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.StatusId = newStatusId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync()
    {
        return await _context.OrderStatuses.ToListAsync();
    }
}
