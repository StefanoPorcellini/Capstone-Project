namespace GestioneOrdini.Model.PriceList
{
    public class LaserPriceList
    {
        public int Id { get; set; }
        public int MinQuantity { get; set; } // Quantità minima
        public int? MaxQuantity { get; set; } // Nullable per gestire il range aperto
        public decimal UnitPrice { get; set; } // Prezzo unitario per questo range

        // Optional: A description to identify the range, if needed.
        public string Description { get; set; }
    }
}
