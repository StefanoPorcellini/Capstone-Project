using GestioneOrdini.Model.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Interface
{
    public interface ICustomerService
    {
        // CRUD generali per i customer
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int customerId);

        // Specifiche operazioni per CustomerCompany e CustomerPrivate
        Task<CustomerCompany> CreateCustomerCompanyAsync(CustomerCompany customerCompany);
        Task<CustomerPrivate> CreateCustomerPrivateAsync(CustomerPrivate customerPrivate);
        Task<CustomerCompany?> GetCustomerCompanyByIdAsync(int customerId);
        Task<CustomerPrivate?> GetCustomerPrivateByIdAsync(int customerId);
    }
}
