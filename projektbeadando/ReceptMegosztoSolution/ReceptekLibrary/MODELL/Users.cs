using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL
{
    public class Users
    {
        [Key]
        public int user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int role_id { get; set; }
    }
}
