using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class User_Default_Tags
    {
        [Key]
        public int user_default_tags_id { get; set; }
        public int user_id { get; set; }
        public int tag_id { get; set; }
    }
}
