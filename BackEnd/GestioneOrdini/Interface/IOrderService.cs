using GestioneOrdini.Model.Order;

namespace GestioneOrdini.Interface
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order, IFormFile? file);
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdateOrderAsync(Order order, IFormFile? file);
        Task DeleteOrderAsync(int id);
        Task AssignOrderToOperatorAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, int newStatusId);
        Task<decimal> CalculateTotalAmountAsync(Order order); // Metodo per calcolare l'importo totale
        Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync();

        Task<(string fileName, string filePath)> UploadFileAsync(IFormFile file);

    }
}
