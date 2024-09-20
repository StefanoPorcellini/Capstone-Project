namespace GestioneOrdini.Model.Order
{
    public abstract class Item
    {
        public int Id { get; set; }
        public int WorkTypeId { get; set; } // FK per WorkType

        public virtual WorkType Type { get; set; } // "Laser" or "Plotter"
        public string WorkDescription { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public virtual Order Order { get; set; }


    }

}
