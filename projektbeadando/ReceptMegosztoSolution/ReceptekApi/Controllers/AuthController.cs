using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptekLibrary.DATA;
using ReceptekLibrary.MODELL;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ReceptekDbContext _context;

    public AuthController(ReceptekDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Users user)
    {
        if (await _context.users.AnyAsync(u => u.username == user.username))
            return BadRequest("User exists");

        user.password = HashPassword(user.password);

        _context.users.Add(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Users user)
    {
        var hash = HashPassword(user.password);

        var exists = await _context.users
            .AnyAsync(u => u.username == user.username && u.password == hash);

        if (!exists)
            return Unauthorized();

        return Ok();
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(
            sha.ComputeHash(Encoding.UTF8.GetBytes(password))
        );
    }
}