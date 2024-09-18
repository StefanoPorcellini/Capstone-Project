using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.Clients;
using GestioneOrdini.Model.ViewModel;
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

        // Metodo per creare un cliente in base al tipo scelto
        public async Task<Customer> CreateCustomerAsync
            (string customerType, string name, string address, string email, string tel, string? cf = null, string? partitaIVA = null, 
            string? ragioneSociale = null)
        {
            Customer customer;

            if (customerType == "company")
            {
                if (string.IsNullOrWhiteSpace(partitaIVA) || string.IsNullOrWhiteSpace(ragioneSociale))
                {
                    throw new ArgumentException("Partita IVA e Ragione Sociale sono richiesti per i clienti aziendali.");
                }

                customer = new CustomerCompany
                {
                    Name = name,
                    Address = address,
                    Email = email,
                    Tel = tel,
                    PartitaIVA = partitaIVA,
                    RagioneSociale = ragioneSociale
                };

                await CreateCustomerCompanyAsync((CustomerCompany)customer);
            }
            else if (customerType == "private")
            {
                
                customer = new CustomerPrivate
                {
                    Name = name,
                    Address = address,
                    Email = email,
                    Tel = tel,
                    CF = cf
                };

                await CreateCustomerPrivateAsync((CustomerPrivate)customer);
            }
            else
            {
                throw new ArgumentException("Tipo di cliente non valido.");
            }

            return customer;
        }

        // Metodi privati per creare clienti specifici
        private async Task<CustomerCompany> CreateCustomerCompanyAsync(CustomerCompany customerCompany)
        {
            _context.Customers.Add(customerCompany); // Aggiungi come Customer
            await _context.SaveChangesAsync();
            return customerCompany;
        }

        private async Task<CustomerPrivate> CreateCustomerPrivateAsync(CustomerPrivate customerPrivate)
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

        // Metodo per ottenere tutti i clienti come ViewModel
        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.ToListAsync();
            return customers.Select(c => ConvertToViewModel(c)).ToList();
        }

        // Metodo privato per convertire un'entità Customer in CustomerViewModel
        private CustomerViewModel ConvertToViewModel(Customer customer)
        {
            if (customer is CustomerCompany company)
            {
                return new CustomerViewModel
                {
                    Id = company.Id,
                    Name = company.Name,
                    Address = company.Address,
                    Email = company.Email,
                    Tel = company.Tel,
                    PartitaIVA = company.PartitaIVA,
                    RagioneSociale = company.RagioneSociale,
                    CustomerType = "company"

                };
            }
            else if (customer is CustomerPrivate privato)
            {
                return new CustomerViewModel
                {
                    Id = privato.Id,
                    Name = privato.Name,
                    Address = privato.Address,
                    Email = privato.Email,
                    Tel = privato.Tel,
                    CF = privato.CF,
                    CustomerType = "private"
                };
            }
            else
            {
                return new CustomerViewModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Address = customer.Address,
                    Email = customer.Email,
                    Tel = customer.Tel,

                };
            }
        }

    }
}
