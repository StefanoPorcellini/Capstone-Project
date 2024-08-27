using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Model.ViewModel
{
    public class CreateUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }  // Password richiesta per la creazione dell'utente

        [Required]
        public string RoleName { get; set; }   // Nome del ruolo associato all'utente

    }
}
