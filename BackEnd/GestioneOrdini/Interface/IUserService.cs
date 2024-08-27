using GestioneOrdini.Model.User;
using GestioneOrdini.Model.ViewModel;

namespace GestioneOrdini.Interface
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(NewUserViewModel user);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);
        Task<User> LoginAsync (string username, string password);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
