//Mark Bernard
using IGDB.Models;
using LudexApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Data
{
    public class LudexDbContext : DbContext
    {
        public LudexDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserGame> UserGames { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ignore ALL IGDB Models – EF should NOT try to map these
            modelBuilder.Ignore<Game>();
            modelBuilder.Ignore<Cover>();
            modelBuilder.Ignore<Platform>();
            modelBuilder.Ignore<Genre>();

            // Table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Post>().ToTable("Posts");
            modelBuilder.Entity<Forum>().ToTable("Forums");
            modelBuilder.Entity<Review>().ToTable("Reviews");

            // Configure UserGame
            modelBuilder.Entity<UserGame>()
                .HasKey(ug => new { ug.UserId, ug.GameId });

            modelBuilder.Entity<UserGame>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.GameLibrary)
                .HasForeignKey(ug => ug.UserId);

            // Configure UserFriend (self-relationship)
            modelBuilder.Entity<UserFriend>()
                .HasKey(uf => new { uf.UserId, uf.FriendId });

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

            // Review → User relation
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            // Review has GameId (no actual Game model)
            modelBuilder.Entity<Review>()
                .Ignore(r => r.Game); // Very important!
        }
    }
}