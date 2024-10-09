namespace GestioneOrdini.Model.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } // Invece di Customer
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? MaxDeliveryDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? OperatorName { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; } // Invece di Status
        public List<ItemWithFileDto> Items { get; set; }
    }
}
