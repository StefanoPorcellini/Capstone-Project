using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.PriceList;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Service
{
    public class LaserPriceListService : ILaserPriceListService
    {
        private readonly OrdersDbContext _context;

        public LaserPriceListService(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<LaserPriceList> CreateLaserPriceListAsync(LaserPriceList priceList)
        {
            _context.LaserPriceLists.Add(priceList);
            await _context.SaveChangesAsync();
            return priceList;
        }

        public async Task<IEnumerable<LaserPriceList>> GetAllLaserPriceListsAsync()
        {
            return await _context.LaserPriceLists.ToListAsync();
        }

        public async Task<LaserPriceList> GetLaserPriceListByIdAsync(int id)
        {
            return await _context.LaserPriceLists.FindAsync(id);
        }

        public async Task UpdateLaserPriceListAsync(LaserPriceList priceList)
        {
            _context.LaserPriceLists.Update(priceList);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLaserPriceListAsync(int id)
        {
            var priceList = await _context.LaserPriceLists.FindAsync(id);
            if (priceList != null)
            {
                _context.LaserPriceLists.Remove(priceList);
                await _context.SaveChangesAsync();
            }
        }
    }
}
