using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestioneOrdini.Service;

namespace GestioneOrdini.Model.User
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Username { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "La password è obbligatoria.")]
        public string Password { get; set; } // Campo per la password in input

        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        [Required]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } // Aggiunto per navigazione

        // Metodo per impostare la password con salt e hash
        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("La password non può essere vuota", nameof(password));
            }

            PasswordSalt = PasswordService.GenerateSalt();
            PasswordHash = PasswordService.HashPassword(password, PasswordSalt);

            if (PasswordHash == null || PasswordSalt == null)
            {
                throw new InvalidOperationException("PasswordHash e PasswordSalt devono essere impostati.");
            }
        }

        // Metodo per verificare la password
        public bool VerifyPassword(string password)
        {
            if (PasswordHash == null || PasswordSalt == null)
            {
                throw new InvalidOperationException("PasswordHash e PasswordSalt devono essere impostati.");
            }

            return PasswordService.VerifyPassword(password, PasswordSalt, PasswordHash);
        }
    }
}
