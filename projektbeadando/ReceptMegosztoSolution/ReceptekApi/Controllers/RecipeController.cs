using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using ReceptekLibrary.DATA;
using Microsoft.EntityFrameworkCore;
using ReceptekLibrary.MODELL;

namespace ReceptekApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly ReceptekDbContext _context;

        public RecipeController(ReceptekDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetRecipes()
        {
            var recipes = await _context.Recipes.ToListAsync();
            return Ok(recipes);
        }

        [HttpGet("Recipes/{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateRecipe(Recipes recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.recipe_id }, recipe);
        }

        [HttpDelete("Recipes/{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("Recipes/{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, Recipes updatedRecipe)
        {
            if (id != updatedRecipe.recipe_id)
            {
                return BadRequest();
            }
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            recipe.title = updatedRecipe.title;
            recipe.description = updatedRecipe.description;
            recipe.prep_time_min = updatedRecipe.prep_time_min;
            recipe.cook_time_min = updatedRecipe.cook_time_min;
            recipe.servings = updatedRecipe.servings;
            _context.Entry(recipe).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(recipe);
        }

        [HttpGet("comments/{recipeId}")]
        public async Task<IActionResult> GetCommentsForRecipe(int recipeId)
        {
            var comments = await _context.comments.Where(c => c.recipe_id == recipeId).ToListAsync();
            return Ok(comments);
        }
    }
}
