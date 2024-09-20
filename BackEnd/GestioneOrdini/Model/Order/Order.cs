using GestioneOrdini.Model.Clients;

namespace GestioneOrdini.Model.Order
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? MaxDeliveryDate { get; set; } 
        public decimal TotalAmount { get; set; }

        public string? OperatorName { get; set; } // Nome dell'operatore che gestisce l'ordine
        public int StatusId { get; set; } // ID dello stato dell'ordine

        public virtual OrderStatus Status { get; set; } // Stato dell'ordine
        public virtual ICollection<Item> Items { get; set; }
    }
}
