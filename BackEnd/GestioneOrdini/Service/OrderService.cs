using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Service
{
    public class OrderService : IOrderService
    {
        private readonly OrdersDbContext _context;

        public OrderService(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            // Calcola il totale prima di salvare l'ordine
            order.TotalAmount = await CalculateTotalAmountAsync(order);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Item) // Includi l'Item correlato
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Item) // Includi gli Item correlati
                .ToListAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            // Calcola nuovamente il totale prima di aggiornare l'ordine
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
                    // Trova il listino prezzi che corrisponde alla quantità per il laser custom
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
                    // Utilizza il prezzo fisso del LaserStandard
                    var laserStandard = await _context.LaserStandards.FindAsync(laserItem.LaserStandardId.Value);
                    if (laserStandard != null)
                    {
                        return laserStandard.Price * laserItem.Quantity.Value;
                    }
                }
            }

            // Logica per altri tipi di item (PlotterItem)
            else if (order.Item is PlotterItem plotterItem)
            {
                if (plotterItem.PlotterStandardId.HasValue)
                {
                    var plotterStandard = await _context.PlotterStandards.FindAsync(plotterItem.PlotterStandardId.Value);
                    return plotterStandard.Price;
                }
                else
                {
                    // Calcolo per plotter custom
                    var area = (decimal)(plotterItem.Base * plotterItem.Height);
                    return area * (decimal)plotterItem.PricePerSquareMeter;
                }
            }

            return 0;
        }

    }
}
