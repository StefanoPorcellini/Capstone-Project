namespace GestioneOrdini.Model.Order
{
    public class LaserItem : Item
    {
        public int? LaserStandardId { get; set; } // Nullable for custom laser
        public string CustomDescription { get; set; } // Nullable if not required
        public int? Quantity { get; set; } // Nullable for standard

        public virtual LaserStandard LaserStandard { get; set; }
    }

}