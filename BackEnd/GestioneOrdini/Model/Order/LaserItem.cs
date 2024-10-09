using GestioneOrdini.Model.PriceList;

namespace GestioneOrdini.Model.Order
{
    public class LaserItem : Item
    {
        public bool IsLaserCustom { get; set; } // Determina se è un lavoro custom

        public int? LaserStandardId { get; set; } // Nullable for custom laser
        public int? Quantity { get; set; } // Nullable for standard

        public virtual LaserStandard LaserStandard { get; set; }


    }

}