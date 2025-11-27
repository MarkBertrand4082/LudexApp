//Mark Bernard
using LudexApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Data
{
    public class LudexDbContext : DbContext
    {
        public LudexDbContext(DbContextOptions<LudexDbContext> options)
            : base(options)
        {
        }

        // -----------------------------
        // DbSets for all entities
        // -----------------------------
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserGame> UserGames { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Rename tables
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Post>().ToTable("Posts");
            modelBuilder.Entity<Forum>().ToTable("Forums");
            modelBuilder.Entity<Review>().ToTable("Reviews");
            modelBuilder.Entity<UserGame>().ToTable("UserGames");
            modelBuilder.Entity<UserFriend>().ToTable("UserFriends");

            // -----------------------------
            // UserGame (many-to-many)
            // -----------------------------
            modelBuilder.Entity<UserGame>()
                .HasKey(ug => new { ug.UserId, ug.GameId }); // composite key

            modelBuilder.Entity<UserGame>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.GameLibrary)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // No Game entity, so just store GameId
            // modelBuilder.Entity<UserGame>().HasOne(ug => ug.Game).WithMany().HasForeignKey(ug => ug.GameId);

            // -----------------------------
            // UserFriend (self-referencing many-to-many)
            // -----------------------------
            modelBuilder.Entity<UserFriend>()
                .HasKey(uf => new { uf.UserId, uf.FriendId }); // composite key

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.Friend)
                .WithMany()
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            // Forum-Post relationship
            // -----------------------------
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Forum)
                .WithMany(f => f.Posts)
                .HasForeignKey(p => p.ForumId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------
            // User-Post relationship
            // -----------------------------
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------
            // Review relationships
            // -----------------------------
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // No Game entity for EF, just GameId integer
            // modelBuilder.Entity<Review>().HasOne(r => r.Game).WithMany().HasForeignKey(r => r.GameId);
        }
    }
}