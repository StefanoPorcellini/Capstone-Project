namespace GestioneOrdini.Model.Order
{
    public class OrderCreationDto
    {
        public int CustomerId { get; set; }

        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? MaxDeliveryDate { get; set; }
        public List<ItemWithFileDto> ItemsWithFiles { get; set; }

    }
}
