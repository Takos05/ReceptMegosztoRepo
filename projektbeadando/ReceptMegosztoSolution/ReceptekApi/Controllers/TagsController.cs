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

        [HttpGet("UserDefaults")]
        public IActionResult GetUserDefaultTags(int userId)
        {
            var tagIds = _context.user_Default_Tags.Where(t => t.user_id == userId).ToList();
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
    }
}
