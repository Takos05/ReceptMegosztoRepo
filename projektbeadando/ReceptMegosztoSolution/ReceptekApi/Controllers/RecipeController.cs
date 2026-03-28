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

        [HttpPost("withTags")]
        public async Task<IActionResult> CreateRecipeWithTags([FromBody] CreateRecipeWithTagsRequest request)
        {
            if (request == null)
            {
                return BadRequest("Érvénytelen kérés.");
            }

            var recipe = new Recipes
            {
                user_id = request.user_id,
                title = request.title,
                description = request.description,
                prep_time_min = request.prep_time_min,
                cook_time_min = request.cook_time_min,
                servings = request.servings,
                created_at = DateTime.Now
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            if (request.tag_ids != null && request.tag_ids.Any())
            {
                var recipeTags = request.tag_ids
                    .Distinct()
                    .Select(tagId => new Recipe_Tags
                    {
                        recipe_id = recipe.recipe_id,
                        tag_id = tagId
                    })
                    .ToList();

                _context.recipe_Tags.AddRange(recipeTags);
                await _context.SaveChangesAsync();
            }

            return Ok(recipe);
        }

        public class CreateRecipeWithTagsRequest
        {
            public int user_id { get; set; }
            public string title { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public int prep_time_min { get; set; }
            public int cook_time_min { get; set; }
            public int servings { get; set; }
            public List<int> tag_ids { get; set; } = new();
        }

        [HttpDelete("{recipeId}")]
        public IActionResult DeleteRecipe(int recipeId)
        {
            var recipe = _context.Recipes.FirstOrDefault(r => r.recipe_id == recipeId);

            if (recipe == null)
            {
                return NotFound("A recept nem található.");
            }

            var recipeTags = _context.recipe_Tags.Where(rt => rt.recipe_id == recipeId).ToList();
            if (recipeTags.Any())
            {
                _context.recipe_Tags.RemoveRange(recipeTags);
            }


            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return Ok();
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

        [HttpGet("byUser/{userId}")]
        public IActionResult GetRecipesByUser(int userId)
        {
            var recipes = _context.Recipes
                .Where(r => r.user_id == userId)
                .OrderByDescending(r => r.recipe_id)
                .ToList();

            return Ok(recipes);
        }
        [HttpGet("{recipeId}/tags")]
        public IActionResult GetTagsForRecipe(int recipeId)
        {
            var tags = _context.recipe_Tags
                .Where(rt => rt.recipe_id == recipeId)
                .Select(rt => rt.tag_id)
                .ToList();

            return Ok(tags);
        }
        [HttpPut("{recipeId}/tags")]
        public async Task<IActionResult> UpdateTags(int recipeId, [FromBody] List<int> tagIds)
        {
            var existingTags = _context.recipe_Tags.Where(rt => rt.recipe_id == recipeId);
            _context.recipe_Tags.RemoveRange(existingTags);

            if (tagIds != null && tagIds.Any())
            {
                var newTags = tagIds.Select(tagId => new Recipe_Tags
                {
                    recipe_id = recipeId,
                    tag_id = tagId
                });

                _context.recipe_Tags.AddRange(newTags);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("allTags")]
        public IActionResult GetAllTags()
        {
            var allTags = _context.tags
                .Select(t => new { t.tag_id, t.tag_name })
                .ToList();
            return Ok(allTags);
        }
    }
}
