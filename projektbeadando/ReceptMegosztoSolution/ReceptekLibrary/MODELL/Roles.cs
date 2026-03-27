using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class Roles
    {
        [Key]
        public int role_id { get; set; }
        public string name { get; set; }
    }
}
