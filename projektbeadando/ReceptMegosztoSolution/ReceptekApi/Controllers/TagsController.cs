using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceptekLibrary.DATA;
using ReceptekLibrary.MODELL;

namespace ReceptekApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ReceptekDbContext _context;

        public TagsController(ReceptekDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public IActionResult GetAllTags()
        {
            var tags = _context.tags
                .OrderBy(t => t.category_id)
                .ThenBy(t => t.tag_name)
                .ToList();

            return Ok(tags);
        }

        [HttpGet("byCategories")]
        public IActionResult GetTagsByCategories()
        {
            var tags = _context.tags.GroupBy(t => t.category_id).ToList();
            var categories = _context.tag_Categories.ToList();
            var result = new Dictionary<string, string[]>();
            foreach (var category in tags)
            {
                var categoryName = categories.FirstOrDefault(c => c.category_id == category.Key);
                string[] temp = new string[category.Count()];
                int i = 0;
                foreach (var tag in category)
                {
                    temp[i] = tag.tag_name;
                    i++;
                }
                result.Add(categoryName.name, temp);
            }
            return Ok(result);
        }

        [HttpGet("byRecipe/{recipeId}")]
        public IActionResult GetTagsByRecipe(int recipeId)
        {
            var tagIds = _context.recipe_Tags.Where(t => t.recipe_id == recipeId).ToList();
            var tags = new List<Tags>();
            foreach (var tagId in tagIds)
            {
                var tag = _context.tags.FirstOrDefault(t => t.tag_id == tagId.tag_id);
                if (tag != null)
                {
                    tags.Add(tag);
                }
            }
            return Ok(tags);
        }

        [HttpGet("UserDefaults")]
        public IActionResult GetUserDefaultTags([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Érvénytelen userId.");
            }

            var defaultTagIds = _context.user_Default_Tags
                .Where(x => x.user_id == userId)
                .Select(x => x.tag_id)
                .ToList();

            var tags = _context.tags
                .Where(t => defaultTagIds.Contains(t.tag_id))
                .OrderBy(t => t.category_id)
                .ThenBy(t => t.tag_name)
                .ToList();

            return Ok(tags);
        }

        [HttpPost("UserDefaults")]
        public IActionResult SaveUserDefaultTags([FromBody] SaveUserDefaultTagsRequest request)
        {
            if (request == null || request.UserId <= 0)
            {
                return BadRequest("Érvénytelen kérés.");
            }

            var existing = _context.user_Default_Tags
                .Where(x => x.user_id == request.UserId)
                .ToList();

            if (existing.Any())
            {
                _context.user_Default_Tags.RemoveRange(existing);
            }

            if (request.TagIds != null && request.TagIds.Any())
            {
                var newItems = request.TagIds
                    .Distinct()
                    .Select(tagId => new User_Default_Tags
                    {
                        user_id = request.UserId,
                        tag_id = tagId
                    })
                    .ToList();

                _context.user_Default_Tags.AddRange(newItems);
            }

            _context.SaveChanges();

            return Ok();
        }


        public class SaveUserDefaultTagsRequest
        {
            public int UserId { get; set; }
            public List<int> TagIds { get; set; } = new();
        }
    }
}
