using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceptekLibrary.DATA;
using ReceptekLibrary.MODELL;

namespace ReceptekApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ReceptekDbContext _context;

        public CommentController(ReceptekDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comments comment)
        {
            _context.comments.Add(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
