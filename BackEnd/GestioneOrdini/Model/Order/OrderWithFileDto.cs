namespace GestioneOrdini.Model.Order
{
    public class OrderWithFileDto
    {
        public Order Order { get; set; }
        public IFormFile? File { get; set; } // Gestisce l'upload del file

    }
}
