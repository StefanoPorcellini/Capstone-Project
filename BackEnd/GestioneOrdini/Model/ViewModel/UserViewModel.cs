using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Model.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }            // ID dell'utente

        [Required]
        public string Username { get; set; }

        [Required]
        public string RoleName { get; set; } 
    }
}
