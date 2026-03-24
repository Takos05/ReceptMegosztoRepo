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

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Users login)
    {
        var hash = HashPassword(login.password);

        var user = await _context.users
            .FirstOrDefaultAsync(u => u.username == login.username && u.password == hash);

        if (user == null)
            return Unauthorized();

        var roles = new List<string> { "User" };

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