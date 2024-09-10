using GestioneOrdini.Interface;
using GestioneOrdini.Model.User;
using GestioneOrdini.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace GestioneOrdini.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _key;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _issuer = configuration["Jwt:Issuer"]!;
            _audience = configuration["Jwt:Audience"]!;
            _key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        }

        // Metodo per la creazione di un nuovo utente
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.CreateUserAsync(model);
                    return Ok(new { Message = "User created successfully." });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating user: {ex.Message}");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }

        // Metodo per ottenere tutti gli utenti
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error retrieving users: {ex.Message}" });
            }
        }

        // Metodo per ottenere un utente per ID
        [HttpGet("getUserById/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound(new { Message = "User not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error retrieving user: {ex.Message}" });
            }
        }

        // Metodo per aggiornare un utente

        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserViewModel user)
        {
            if (userId != user.Id)
            {
                return BadRequest(new { Message = "User ID mismatch." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateUserAsync(user);
                    return Ok(new { Message = "User updated successfully." });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating user: {ex.Message}");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }


        // Metodo per eliminare un utente
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error deleting user: {ex.Message}" });
            }
        }

        // Metodo di login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.LoginAsync(model.Username, model.Password);
                    if (user != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, user.RoleId.ToString())
                        };

                        var key = new SymmetricSecurityKey(_key);
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var expiration = DateTime.Now.AddYears(1);

                        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                            issuer: _issuer,
                            audience: _audience,
                            claims: claims,
                            expires: expiration,
                            signingCredentials: creds
                        );

                        return Ok(new LoginResponseModel
                        {
                            Id = user.Id,
                            Username = user.Username,
                            Expires = expiration,
                            Token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token)
                        });
                    }
                    return Unauthorized(new { Message = "Invalid username or password." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = $"Error during login: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _userService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error retrieving roles: {ex.Message}" });
            }
        }

    }
}
