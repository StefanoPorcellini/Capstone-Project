namespace GestioneOrdini.Model.Order
{
    public abstract class Item
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Laser" or "Plotter"
        public string WorkDescription { get; set; }

        // Discriminator to determine if the item is Laser or Plotter
        public string Discriminator { get; set; }
    }

}
