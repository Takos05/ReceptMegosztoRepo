using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class Tags
    {
        [Key]
        public int tag_id { get; set; }
        public string tag_name { get; set; }
        public int category_id { get; set; }
    }
}
