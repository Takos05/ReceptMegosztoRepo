using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class Recipes
    {
        [Key]
        public int recipe_id { get; set; }
        public int user_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int prep_time_min { get; set; }
        public int cook_time_min { get; set; }
        public int servings { get; set; }
        public DateTime created_at { get; set; }

    }
}
