using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class OrderService : IOrderService
{
    private readonly OrdersDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _storagePath = "~/FileUpload"; 

    public OrderService(OrdersDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Order> CreateOrderAsync(Order order, IFormFile? file)
    {
        order.StatusId = 1;
        order.TotalAmount = await CalculateTotalAmountAsync(order);

        // Upload del file durante la creazione dell'ordine
        if (file != null && order.Items.Any())
        {
            var (fileName, filePath) = await UploadFileAsync(file);
            foreach (var item in order.Items)
            {
                item.FileName = fileName;
                item.FilePath = filePath;
            }
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Status)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Status)
            .Include(o => o.Customer)
            .ToListAsync();
    }

    public async Task UpdateOrderAsync(Order order, IFormFile? file)
    {
        // Aggiorna il file se fornito durante la modifica
        if (file != null && order.Items.Any())
        {
            var (fileName, filePath) = await UploadFileAsync(file);
            foreach (var item in order.Items)
            {
                item.FileName = fileName;
                item.FilePath = filePath;
            }
        }

        order.TotalAmount = await CalculateTotalAmountAsync(order);
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null)
        {
            // Cancella i file associati, se esistenti
            foreach (var item in order.Items)
            {
                if (!string.IsNullOrEmpty(item.FilePath) && File.Exists(item.FilePath))
                {
                    File.Delete(item.FilePath);
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> CalculateTotalAmountAsync(Order order)
    {
        decimal totalAmount = 0;

        foreach (var item in order.Items)
        {
            if (item is LaserItem laserItem)
            {
                if (laserItem.IsCustom)
                {
                    var priceList = await _context.LaserPriceLists
                        .Where(lp => lp.MinQuantity <= laserItem.Quantity &&
                                     (lp.MaxQuantity == null || laserItem.Quantity <= lp.MaxQuantity))
                        .FirstOrDefaultAsync();

                    if (priceList != null)
                    {
                        totalAmount += priceList.UnitPrice * laserItem.Quantity.Value;
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
                        totalAmount += laserStandard.Price * laserItem.Quantity.Value;
                    }
                }
            }
            else if (item is PlotterItem plotterItem)
            {
                if (plotterItem.IsCustom)
                {
                    var area = (decimal)(plotterItem.Base * plotterItem.Height);
                    totalAmount += area * (decimal)plotterItem.PricePerSquareMeter * plotterItem.Quantity.Value;
                }
                else if (plotterItem.PlotterStandardId.HasValue)
                {
                    var plotterStandard = await _context.PlotterStandards.FindAsync(plotterItem.PlotterStandardId.Value);
                    totalAmount += plotterStandard.Price * plotterItem.Quantity.Value;
                }
            }
        }

        return totalAmount;
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

    // Logica per il caricamento del file
    public async Task<(string fileName, string filePath)> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception("File non valido.");
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(_storagePath, fileName);

        Directory.CreateDirectory(_storagePath); // Assicura che la directory esista

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return (fileName, filePath);
    }
}