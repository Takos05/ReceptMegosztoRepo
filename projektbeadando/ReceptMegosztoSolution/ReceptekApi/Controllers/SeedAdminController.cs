// Fájl helye: ReceptekApi/Controllers/SeedAdminController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptekLibrary.DATA;
using ReceptekLibrary.MODELL;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ReceptekApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedAdminController : ControllerBase
    {
        private readonly ReceptekDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SeedAdminController(ReceptekDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // POST /api/SeedAdmin?userName=admin&email=admin@local&password=Admin123!
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromQuery] string userName = "admin",
            [FromQuery] string email = "admin@local",
            [FromQuery] string password = "Admin123!")
        {
            if (!_environment.IsDevelopment())
            {
                return Forbid("Csak fejlesztési környezetben használható.");
            }

            var existingAdmin = await _context.users
                .FirstOrDefaultAsync(u => u.username == userName || u.email == email);

            if (existingAdmin != null)
            {
                return Ok(new { message = "Admin már létezik." });
            }

            // Admin role lekérdezése
            var adminRole = await _context.roles
                .FirstOrDefaultAsync(r => r.name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Roles
                {
                    name = "Admin"
                };

                _context.roles.Add(adminRole);
                await _context.SaveChangesAsync();
            }

            // Felhasználó létrehozása
            var adminUser = new Users
            {
                username = userName,
                email = email,
                password = HashPassword(password)
            };

            _context.users.Add(adminUser);
            await _context.SaveChangesAsync();

            // Role hozzárendelés (UserRoles tábla)
            var userRole = new UserRole
            {
                user_id = adminUser.user_id,
                role_id = adminRole.role_id
            };

            _context.userRole.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin létrehozva és role hozzárendelve.",
                userName,
                email,
                password
            });
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(password))
            );
        }
    }
}