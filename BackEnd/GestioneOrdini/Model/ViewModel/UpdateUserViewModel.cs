using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Model.ViewModel
{
    public class UpdateUserViewModel
    {
        public int Id { get; set; }

        public string? Username { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        public string RoleName { get; set; } // Il nome del ruolo selezionato
    }
}
