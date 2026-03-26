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
        public DbSet<Recipes> Recipes { get; set; }
        public DbSet<Tags> tags { get; set; }
        public DbSet<Tag_Categories> tag_Categories { get; set; }
        public DbSet<Recipe_Tags> recipe_Tags { get; set; }
        public DbSet<User_Default_Tags> user_Default_Tags { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Ratings> Ratings { get; set; }
        public DbSet<Comments> comments { get; set; }
    }
}
