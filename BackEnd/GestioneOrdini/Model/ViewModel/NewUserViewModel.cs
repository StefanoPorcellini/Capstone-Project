using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Model.ViewModel
{
    public class NewUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
