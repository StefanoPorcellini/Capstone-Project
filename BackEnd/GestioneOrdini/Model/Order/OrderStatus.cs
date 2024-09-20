namespace GestioneOrdini.Model.Order
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

    }
}