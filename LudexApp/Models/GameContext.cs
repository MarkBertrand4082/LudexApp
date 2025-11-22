//Mark Bertrand
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Models
{
    public class GameContext : DbContext
    {

      public GameContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<User>().ToTable("UserTable");
            modelBuilder.Entity<Post>().ToTable("PostTable");
        }
    }
}
