namespace GestioneOrdini.Model.Order
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? MaxDeliveryDate { get; set; } // Nullable if not required
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
    }
}
