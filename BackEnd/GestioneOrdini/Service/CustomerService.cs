using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Clients;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly OrdersDbContext _context;

        public CustomerService(OrdersDbContext context)
        {
            _context = context;
        }

        // Metodo per creare un cliente
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        // Metodi per creare clienti specifici
        public async Task<CustomerCompany> CreateCustomerCompanyAsync(CustomerCompany customerCompany)
        {
            _context.Customers.Add(customerCompany); // Aggiungi come Customer
            await _context.SaveChangesAsync();
            return customerCompany;
        }

        public async Task<CustomerPrivate> CreateCustomerPrivateAsync(CustomerPrivate customerPrivate)
        {
            _context.Customers.Add(customerPrivate); // Aggiungi come Customer
            await _context.SaveChangesAsync();
            return customerPrivate;
        }

        // Metodo per ottenere un cliente per ID
        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        // Metodi per ottenere clienti specifici per ID
        public async Task<CustomerCompany?> GetCustomerCompanyByIdAsync(int customerId)
        {
            return await _context.Customers
                .OfType<CustomerCompany>() // Filtra per CustomerCompany
                .SingleOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<CustomerPrivate?> GetCustomerPrivateByIdAsync(int customerId)
        {
            return await _context.Customers
                .OfType<CustomerPrivate>() // Filtra per CustomerPrivate
                .SingleOrDefaultAsync(c => c.Id == customerId);
        }

        // Metodo per ottenere tutti i clienti
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        // Metodo per aggiornare un cliente
        public async Task UpdateCustomerAsync(Customer customer)
        {
            // Trova il cliente esistente nel database
            var existingCustomer = await _context.Customers
                .AsNoTracking() // Usa AsNoTracking per evitare che EF modifichi l'oggetto recuperato
                .SingleOrDefaultAsync(c => c.Id == customer.Id);

            if (existingCustomer == null)
            {
                throw new ArgumentException("Customer not found.");
            }

            // Aggiorna i campi comuni
            existingCustomer.Name = customer.Name;
            existingCustomer.Address = customer.Address;
            existingCustomer.Email = customer.Email;
            existingCustomer.Tel = customer.Tel;

            // Determina il tipo di cliente e aggiorna i campi specifici
            if (customer is CustomerCompany customerCompany)
            {
                var existingCustomerCompany = existingCustomer as CustomerCompany;
                if (existingCustomerCompany != null)
                {
                    existingCustomerCompany.PartitaIVA = customerCompany.PartitaIVA;
                    existingCustomerCompany.RagioneSociale = customerCompany.RagioneSociale;
                }
            }
            else if (customer is CustomerPrivate customerPrivate)
            {
                var existingCustomerPrivate = existingCustomer as CustomerPrivate;
                if (existingCustomerPrivate != null)
                {
                    existingCustomerPrivate.CF = customerPrivate.CF;
                }
            }

            // Aggiungi l'oggetto modificato al contesto e salva le modifiche
            _context.Customers.Update(existingCustomer);
            await _context.SaveChangesAsync();
        }

        // Metodo per eliminare un cliente
        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
