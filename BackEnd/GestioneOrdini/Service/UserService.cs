using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Model.User;
using GestioneOrdini.Model.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace GestioneOrdini.Service
{
    public class UserService : IUserService
    {
        private readonly OrdersDbContext _context;

        public UserService(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(NewUserViewModel userViewModel)
        {
            try
            {
                // Ottieni il ruolo dal database
                var role = await _context.Roles.FindAsync(userViewModel.RoleId);
                if (role == null)
                {
                    throw new ArgumentException("Invalid role ID provided.");
                }

                var newUser = new User
                {
                    Username = userViewModel.Username,
                    RoleId = userViewModel.RoleId,
                    Role = role
                };

                newUser.SetPassword(userViewModel.Password);

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return newUser;
            }
            catch (Exception ex)
            {
                // Gestione degli errori durante la creazione dell'utente
                throw;
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                var utente = await _context.Users.FindAsync(userId);
                if (utente != null)
                {
                    _context.Users.Remove(utente);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Gestione del caso in cui l'utente non viene trovato per l'eliminazione
                }
            }
            catch (Exception ex)
            {
                // Gestione degli errori durante l'eliminazione dell'utente
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                // Gestione degli errori durante il recupero di tutti gli utenti
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId);
            }
            catch (Exception ex)
            {
                // Gestione degli errori durante la ricerca dell'utente per ID
                throw;
            }
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    // Gestione del caso in cui l'utente non viene trovato
                    return null;
                }

                bool isValid = PasswordService.VerifyPassword(password, user.PasswordSalt, user.PasswordHash);
                if (isValid)
                {
                    return user;
                }
                else
                {
                    // Gestione del caso in cui la password non è valida
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Gestione degli errori durante il login
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Gestione degli errori durante l'aggiornamento dell'utente
                throw;
            }
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            try
            {
                return await _context.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                // Gestione degli errori durante il recupero di tutti i ruoli
                throw;
            }
        }

    }
}
