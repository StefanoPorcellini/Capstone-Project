using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class OrderService : IOrderService
{
    private readonly OrdersDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _storagePath;

    public OrderService(OrdersDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _storagePath = Path.Combine(env.ContentRootPath, "FileUpload"); // Percorso fisico per i file
        if(string.IsNullOrEmpty(_storagePath))
        {
            throw new ArgumentNullException(nameof(_storagePath), "The file upload path cannot be null or empty.");
        }
    }

    // Creazione di un ordine e gestione del caricamento dei file a livello di item
    public async Task<Order> CreateOrderAsync(Order order, List<ItemWithFileDto> itemsWithFiles)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            order.StatusId = 1;
            order.Items = new List<Item>();

            foreach (var itemWithFile in itemsWithFiles ?? new List<ItemWithFileDto>())
            {
                Item newItem;
                if (itemWithFile.ItemId > 0)
                {
                    newItem = await _context.Items.FindAsync(itemWithFile.ItemId) ?? throw new Exception($"Item con ID {itemWithFile.ItemId} non trovato.");
                }
                else
                {
                    if (itemWithFile.WorkTypeId == 1)
                    {
                        // Assicurati che ItemWithFileDto contenga le proprietà necessarie
                        newItem = new LaserItem
                        {
                            WorkDescription = itemWithFile.WorkDescription ?? "", // Aggiunto per sicurezza
                            LaserStandardId = itemWithFile.LaserStandardId, // Aggiungi altre proprietà di LaserItem
                            Quantity = itemWithFile.Quantity,             // Aggiungi altre proprietà di LaserItem
                            IsLaserCustom = itemWithFile.IsLaserCustom    // Aggiungi altre proprietà di LaserItem
                        };
                    }
                    else if (itemWithFile.WorkTypeId == 2)
                    {
                        newItem = new PlotterItem
                        {
                            WorkDescription = itemWithFile.WorkDescription ?? "", // Aggiunto per sicurezza
                            PlotterStandardId = itemWithFile.PlotterStandardId, // Aggiungi altre proprietà di PlotterItem
                            Base = itemWithFile.Base,                         // Aggiungi altre proprietà di PlotterItem
                            Height = itemWithFile.Height,                       // Aggiungi altre proprietà di PlotterItem
                            Quantity = itemWithFile.Quantity,                   // Aggiungi altre proprietà di PlotterItem
                            IsPlotterCustom = itemWithFile.IsPlotterCustom     // Aggiungi altre proprietà di PlotterItem
                        };
                    }
                    else
                    {
                        throw new Exception($"WorkTypeId {itemWithFile.WorkTypeId} non valido.");
                    }
                    newItem.WorkTypeId = itemWithFile.WorkTypeId;
                }

                if (itemWithFile.File != null)
                {
                    var (fileName, filePath) = await UploadFileAsync(itemWithFile.File);
                    newItem.FileName = fileName;
                    newItem.FilePath = filePath;
                }
                newItem.OrderId = order.Id;
                order.Items.Add(newItem);
            }

            order.TotalAmount = await CalculateTotalAmountAsync(order);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Errore durante la creazione dell'ordine: {ex}");
            throw new Exception("Errore durante la creazione dell'ordine. Controlla i dati inseriti e riprova.", ex);
        }

        return order;
    }

    // Aggiornamento dell'ordine e gestione del file a livello di item
    public async Task UpdateOrderAsync(Order order, List<ItemWithFileDto> itemsWithFiles)
    {
        // Aggiorna il file e l'item associato
        foreach (var itemWithFile in itemsWithFiles)
        {
            var existingItem = await _context.Items.FindAsync(itemWithFile.ItemId);
            if (existingItem == null)
            {
                throw new Exception("Item non trovato.");
            }

            if (itemWithFile.File != null)
            {
                var (fileName, filePath) = await UploadFileAsync(itemWithFile.File);
                existingItem.FileName = fileName;
                existingItem.FilePath = filePath;
            }

            // Aggiorna le proprietà dell'item esistente
            _context.Entry(existingItem).CurrentValues.SetValues(existingItem);
        }

        order.TotalAmount = await CalculateTotalAmountAsync(order);
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
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

    // Eliminazione dell'ordine e cancellazione dei file associati agli item
    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => (i as LaserItem).LaserStandard)
            .Include(o => o.Items)
                .ThenInclude(i => (i as PlotterItem).PlotterStandard)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null)
        {
            // Delete associated files for each item
            foreach (var item in order.Items)
            {
                if (!string.IsNullOrEmpty(item.FilePath) && File.Exists(item.FilePath))
                {
                    File.Delete(item.FilePath);
                }
            }

            _context.Items.RemoveRange(order.Items);

            // Remove the order
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"Order with ID {id} not found.");
        }
    }

    public async Task<decimal> CalculateTotalAmountAsync(Order order)
    {
        decimal totalAmount = 0;

        if (order.Items == null) return 0; // oppure lancia un'eccezione, a seconda della logica desiderata


        foreach (var item in order.Items)
        {
            if (item is LaserItem laserItem)
            {
                if (laserItem.IsLaserCustom)
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
                if (plotterItem.IsPlotterCustom)
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

    public async Task<(string fileName, string filePath)> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception("File non valido.");
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(_storagePath, fileName);

        try
        {
            Directory.CreateDirectory(_storagePath); // Assicura che la directory esista

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Errore durante il caricamento del file: " + ex.Message);
        }

        return (fileName, filePath);
    }

    public async Task<IEnumerable<WorkType>> GetAllWorkTypesAsync()
    {
        return await _context.WorkTypes.ToListAsync();
    }
}
