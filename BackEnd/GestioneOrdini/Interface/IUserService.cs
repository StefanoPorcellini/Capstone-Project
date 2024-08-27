using GestioneOrdini.Model.User;
using GestioneOrdini.Model.ViewModel;

namespace GestioneOrdini.Interface
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UpdateUserViewModel user);           // Metodo per creare un nuovo utente
        Task<UserViewModel> GetUserByIdAsync(int userId);               // Metodo per ottenere un utente per ID
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync();            // Metodo per ottenere tutti gli utenti con il ruolo associato
        Task UpdateUserAsync(UpdateUserViewModel user);                 // Metodo per aggiornare un utente esistente
        Task DeleteUserAsync(int userId);                               // Metodo per eliminare un utente
        Task<User> LoginAsync(string username, string password);        // Metodo per eseguire il login di un utente
        Task<IEnumerable<Role>> GetAllRolesAsync();                     // Metodo per ottenere tutti i ruoli

    }
}
