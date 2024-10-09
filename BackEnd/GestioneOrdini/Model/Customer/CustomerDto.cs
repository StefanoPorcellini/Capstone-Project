namespace GestioneOrdini.Model.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string Tel { get; set; }
        public string? PartitaIVA { get; set; }
        public string? RagioneSociale { get; set; }
        public string? CF { get; set; }

    }
}
