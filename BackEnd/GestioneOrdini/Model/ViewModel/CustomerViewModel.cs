namespace GestioneOrdini.Model.ViewModel
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public required string Tel { get; set; }
        public string? CF { get; set; }

        public  string? PartitaIVA { get; set; }
        public  string? RagioneSociale { get; set; }
        public string CustomerType { get; set }

    }
}
