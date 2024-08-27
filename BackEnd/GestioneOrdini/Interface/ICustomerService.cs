using GestioneOrdini.Model.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Interface
{
    public interface ICustomerService
    {
        // CRUD generali per i customer
        Task<Customer> CreateCustomerAsync
                    (string customerType, string name, string address, string email, string tel, string? cf = null, string? partitaIVA = null,
                    string? ragioneSociale = null);
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int customerId);

        // Specifiche operazioni per CustomerCompany e CustomerPrivate
        Task<CustomerCompany?> GetCustomerCompanyByIdAsync(int customerId);
        Task<CustomerPrivate?> GetCustomerPrivateByIdAsync(int customerId);
    }
}
