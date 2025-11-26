//Mark Bertrand
using LudexApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Data
{
    public class LudexDbContext : DbContext
    {

      public LudexDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<User>().ToTable("UserTable");
            modelBuilder.Entity<Post>().ToTable("PostTable");
            modelBuilder.Entity<Review>().ToTable("ReviewTable");
            modelBuilder.Entity<Forum>().ToTable("ForumTable");
        }
    }
}
