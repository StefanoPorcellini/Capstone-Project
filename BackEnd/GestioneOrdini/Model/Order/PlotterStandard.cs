namespace GestioneOrdini.Model.Order
{
    public class PlotterStandard
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } 
        public virtual ICollection<PlotterItem> PlotterItems { get; set; }

    }

}
