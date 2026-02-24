using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class Comments
    {
        [Key]
        public int comment_id { get; set; }
        public int user_id { get; set; }
        public int recipe_id { get; set; }
        public string comment_text { get; set; }
        public DateTime created_at { get; set; }
    }
}
