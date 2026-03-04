using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceptekLibrary.DATA;

namespace ReceptekApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly ReceptekDbContext _context;

        public PingController(ReceptekDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Ping()
        {
            var valasz = await _context.Database.CanConnectAsync();

            if (valasz)
            {
                return Ok("Kapcsolat");
            }
            else return StatusCode(StatusCodes.Status503ServiceUnavailable, "Nincs kapcsolat");
        }
    }
}
