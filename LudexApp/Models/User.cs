using IGDB.Models;
using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models
{
    public class User
    {
        public User()
        {
            Friends = new List<UserFriend>();
            GameLibrary = new List<UserGame>();
            GameReviews = new List<UserReview>();
            Posts = new List<Post>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // hashed later


        // ---------------------------
        // Friends (User ↔ User)
        // ---------------------------
        public List<UserFriend> Friends { get; set; }

        // ---------------------------
        // Game Library (User ↔ Game)
        // ---------------------------
        public List<UserGame> GameLibrary { get; set; }

        // ---------------------------
        // Reviews (User ↔ Review)
        // ---------------------------
        public List<UserReview> GameReviews { get; set; }

        // ---------------------------
        // Posts (User → Post)
        // ---------------------------
        public List<Post> Posts { get; set; }
    }

    public class UserFriend
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int FriendId { get; set; }
        public User Friend { get; set; }
    }

    public class UserGame
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }

    public class UserReview
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ReviewId { get; set; }
        public Review Review { get; set; }
    }
}