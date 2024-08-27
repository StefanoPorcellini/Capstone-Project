using GestioneOrdini.Model.Order;

namespace GestioneOrdini.Model.PriceList
{
    public class PriceListItem
    {
        public int Id { get; set; }
        public int PriceListId { get; set; }
        public virtual PriceList PriceList { get; set; }
        public int DimensionId { get; set; }
        public virtual Item Dimension { get; set; }
        public int TypeId { get; set; }
        public virtual LaserItem OrderType { get; set; }
        public int MinQuantity { get; set; } // Quantità minima per il prezzo
        public int MaxQuantity { get; set; } // Quantità massima per il prezzo
        public decimal UnitPrice { get; set; } // Prezzo per unità

    }
}
