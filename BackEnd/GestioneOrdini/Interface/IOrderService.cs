using GestioneOrdini.Model.Order;

namespace GestioneOrdini.Interface
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task<decimal> CalculateTotalAmountAsync(Order order); // Metodo per calcolare l'importo totale

    }
}
