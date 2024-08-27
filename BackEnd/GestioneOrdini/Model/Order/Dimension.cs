namespace GestioneOrdini.Model.Order
{
    public class Dimension
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
