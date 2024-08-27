namespace GestioneOrdini.Model.Clients
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public required string Tel { get; set; }

    }
}
