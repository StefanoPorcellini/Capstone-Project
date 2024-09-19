namespace GestioneOrdini.Model.ViewModel
{
    public class CreateCustomerRequest
    {
        public string CustomerType { get; set; } // "Private" or "Company"
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string Tel { get; set; }
        public string? CF { get; set; } // Required if Private
        public string? PartitaIVA { get; set; } // Required if Company
        public string? RagioneSociale { get; set; } // Required if Company
    }
}
