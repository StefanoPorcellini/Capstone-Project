namespace GestioneOrdini.Model.Order
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? MaxDeliveryDate { get; set; } 
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }

        public decimal TotalAmount { get; set; }

        public string? OperatorName { get; set; } // Nome dell'operatore che gestisce l'ordine
        public int StatusId { get; set; } // ID dello stato dell'ordine

        public virtual OrderStatus Status { get; set; } // Stato dell'ordine


    }
}
