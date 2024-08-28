using GestioneOrdini.Model.PriceList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Interface
{
    public interface ILaserPriceListService
    {
        Task<LaserPriceList> CreateLaserPriceListAsync(LaserPriceList priceList);
        Task<IEnumerable<LaserPriceList>> GetAllLaserPriceListsAsync();
        Task<LaserPriceList> GetLaserPriceListByIdAsync(int id);
        Task UpdateLaserPriceListAsync(LaserPriceList priceList);
        Task DeleteLaserPriceListAsync(int id);
    }
}
