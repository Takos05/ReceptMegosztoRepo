using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptekLibrary.DATA;
using ReceptekLibrary.MODELL;
using ReceptekApi.API.AuthService;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ReceptekDbContext _context;
    private readonly JWTTokenService _jwt;

    public AuthController(ReceptekDbContext context, JWTTokenService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Users user)
    {
        if (await _context.users.AnyAsync(u => u.username == user.username))
            return BadRequest("User exists/ A felhasználó létezik");

        user.password = HashPassword(user.password);

        _context.users.Add(user);
        await _context.SaveChangesAsync();

        var userRoleEntity = await _context.roles
            .FirstOrDefaultAsync(r => r.name == "User");

        if (userRoleEntity == null)
        {
            userRoleEntity = new Roles
            {
                name = "User"
            };

            _context.roles.Add(userRoleEntity);
            await _context.SaveChangesAsync();
        }

        // kapcsolat létrehozása
        var userRole = new UserRole
        {
            user_id = user.user_id,
            role_id = userRoleEntity.role_id,
        };

        _context.userRole.Add(userRole);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Users login)
    {
        var hash = HashPassword(login.password);

        var user = await _context.users
            .FirstOrDefaultAsync(u => u.username == login.username && u.password == hash);

        if (user == null)
            return Unauthorized("Sikertelen bejelentkezés");

        var roles = await (
        from ur in _context.userRole
        join r in _context.roles on ur.role_id equals r.role_id
        where ur.user_id == user.user_id
        select r.name
        ).ToListAsync();

        var token = _jwt.CreateToken(user, roles);

        return Ok(new
        {
            token = token
        });
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(
            sha.ComputeHash(Encoding.UTF8.GetBytes(password))
        );
    }
}