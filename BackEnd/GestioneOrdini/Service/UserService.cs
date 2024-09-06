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

        // Metodo per creare un nuovo utente
        public async Task<User> CreateUserAsync(CreateUserViewModel userViewModel)
        {
            try
            {
                // Trova il ruolo corrispondente al nome del ruolo selezionato
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == userViewModel.RoleName);
                if (role == null)
                {
                    throw new ArgumentException("Role not found.");
                }

                var newUser = new User
                {
                    Username = userViewModel.Username,
                    RoleId = role.Id,
                    Role = role
                };

                newUser.SetPassword(userViewModel.Password);

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return newUser;
            }
            catch (Exception ex)
            {
                // Aggiungi qui logica per gestire gli errori, es. logging
                throw new Exception("An error occurred while creating the user.", ex);
            }
        }

        // Metodo per eliminare un utente
        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentException("User not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }

        // Metodo per ottenere tutti gli utenti
        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Role)  // Include il ruolo nei risultati
                    .ToListAsync();

                return users.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    RoleName = u.Role.Name
                });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }

        // Metodo per ottenere un utente per ID
        public async Task<UserViewModel> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)  // Include il ruolo nei risultati
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return null;
                }

                return new UserViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    RoleName = user.Role.Name
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
        }

        // Metodo di login
        public async Task<User> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)  // Include il ruolo nei risultati
                    .SingleOrDefaultAsync(u => u.Username == username);

                if (user == null || !PasswordService.VerifyPassword(password, user.PasswordSalt!, user.PasswordHash!))
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during login.", ex);
            }
        }

        // Metodo per aggiornare un utente esistente
        public async Task UpdateUserAsync(UpdateUserViewModel userViewModel)
        {
            try
            {
                // Trova l'utente da aggiornare
                var user = await _context.Users.FindAsync(userViewModel.Id);
                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }

                // Aggiorna il nome utente
                user.Username = userViewModel.Username!;

                // Aggiorna la password solo se è stata fornita una nuova password
                if (!string.IsNullOrEmpty(userViewModel.Password))
                {
                    user.SetPassword(userViewModel.Password);
                }

                // Trova il ruolo corrispondente al nome del ruolo selezionato
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == userViewModel.RoleName);
                if (role == null)
                {
                    throw new ArgumentException("Role not found.");
                }

                // Aggiorna l'ID del ruolo
                user.RoleId = role.Id;

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }

        // Metodo per ottenere tutti i ruoli
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            try
            {
                return await _context.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving roles.", ex);
            }
        }
    }
}
