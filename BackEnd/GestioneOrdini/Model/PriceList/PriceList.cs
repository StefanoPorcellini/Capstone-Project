namespace GestioneOrdini.Model.PriceList
{
    public class PriceList
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Nullable, se il listino non ha una data di fine
        public virtual ICollection<PriceListItem> PriceListItems { get; set; } // Collezione di articoli del listino

    }
}
