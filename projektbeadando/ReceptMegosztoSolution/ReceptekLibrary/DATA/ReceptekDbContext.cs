using Microsoft.EntityFrameworkCore;
using ReceptekLibrary.MODELL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptekLibrary.DATA
{
    public class ReceptekDbContext : DbContext
    {
        public ReceptekDbContext(DbContextOptions<ReceptekDbContext> options) : base(options)
        {

        }
        DbSet<Recipes> recipes { get; set; }
        DbSet<Tags> tags { get; set; }
        DbSet<Tag_Categories> tag_Categories { get; set; }
        DbSet<Recipe_Tags> recipe_Tags { get; set; }
        DbSet<User_Default_Tags> user_Default_Tags { get; set; }
        DbSet<Users> users { get; set; }
        DbSet<Ratings> ratings { get; set; }
        DbSet<Comments> comments { get; set; }
    }
}
