using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class Ratings
    {
        [Key]
        public int rating_id { get; set; }
        public int user_id { get; set; }
        public int recipe_id { get; set; }
        public int ratings_value { get; set; }
        public DateTime created_at { get; set; }
    }
}
