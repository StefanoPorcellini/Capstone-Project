using GestioneOrdini.Model.PriceList;

namespace GestioneOrdini.Model.Order
{
    public class LaserItem : Item
    {
        public bool IsCustom { get; set; } // Determina se è un lavoro custom

        public int? LaserStandardId { get; set; } // Nullable for custom laser
        public string CustomDescription { get; set; } // Nullable if not required
        public int? Quantity { get; set; } // Nullable for standard

        public virtual LaserStandard LaserStandard { get; set; }

        public virtual ICollection<LaserPriceList> LaserPriceLists { get; set; } // Collezione di prezzi per i range

    }

}