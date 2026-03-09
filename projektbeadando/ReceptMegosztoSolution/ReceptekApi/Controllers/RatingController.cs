using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceptekLibrary.DATA;
using ReceptekLibrary.MODELL;

namespace ReceptekApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly ReceptekDbContext _context;

        public RatingController(ReceptekDbContext context)
        {
            _context = context;
        }

        [HttpGet("averageByRecipe")]
        public IActionResult GetAverageRatingByRecipe(int recipeId)
        {
            var ratingsList = _context.ratings.Where(r => r.recipe_id == recipeId).ToList();
            if (ratingsList.Count == 0)
            {
                return Ok(0);
            }
            var averageRating = ratingsList.Average(r => r.ratings_value);
            return Ok(averageRating);
        }

        [HttpPost]
        public IActionResult AddRating(Ratings rating)
        {
            _context.ratings.Add(rating);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteRating(int ratingId)
        {
            var rating = _context.ratings.FirstOrDefault(r => r.rating_id == ratingId);
            if (rating == null)
            {
                return NotFound();
            }
            _context.ratings.Remove(rating);
            _context.SaveChanges();
            return Ok();
        }
    }
}
