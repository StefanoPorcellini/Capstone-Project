namespace GestioneOrdini.Model.Clients
{
    public class CustomerCompany : Customer
    {
        public required string PartitaIVA {  get; set; }
        public required string RagioneSociale { get; set; }
    }
}
