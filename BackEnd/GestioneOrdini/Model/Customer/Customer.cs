using System.Collections.Generic;
using GestioneOrdini.Model.Order;

namespace GestioneOrdini.Model.Clients
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public required string Tel { get; set; }
        public virtual ICollection<GestioneOrdini.Model.Order.Order>? Orders { get; set; }

        public Customer()
        {
            Orders = new List<GestioneOrdini.Model.Order.Order>();
        }


    }
}
