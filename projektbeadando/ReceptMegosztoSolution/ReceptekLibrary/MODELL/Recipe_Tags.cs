using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class Recipe_Tags
    {
        [Key]
        public int recipe_tags_id { get; set; }
        public int recipe_id { get; set; }
        public int tag_id { get; set; }
    }
}
