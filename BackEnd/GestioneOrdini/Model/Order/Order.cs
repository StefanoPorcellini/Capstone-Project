namespace GestioneOrdini.Model.Order
{
    public class Order
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public required int Quantity { get; set; } = 1;
        public required int TypeId { get; set; }
        public int Price { get; set; }
        public int DimensionId { get; set; }
        public virtual Dimension Dimension { get; set; }
    }
}
