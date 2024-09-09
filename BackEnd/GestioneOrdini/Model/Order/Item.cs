namespace GestioneOrdini.Model.Order
{
    public abstract class Item
    {
        public int Id { get; set; }
        public int WorkTypeId { get; set; } // FK per WorkType

        public WorkType Type { get; set; } // "Laser" or "Plotter"
        public string WorkDescription { get; set; }

    }

}
