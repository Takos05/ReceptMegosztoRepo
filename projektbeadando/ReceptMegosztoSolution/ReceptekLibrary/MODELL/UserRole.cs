using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.MODELL;
    public class UserRole
    {
        [Key]
        public int user_id { get; set; }
        public int role_id { get; set; }
    }
