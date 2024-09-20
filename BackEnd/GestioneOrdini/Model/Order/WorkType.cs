namespace GestioneOrdini.Model.Order
{
    public class WorkType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Item> Items { get; set; }

    }
}