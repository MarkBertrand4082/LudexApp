using IGDB.Models;
using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models
{
    public class User
    {
        public User()
        {
            Posts = new List<Post>();
            Reviews = new List<Review>();
            GameLibrary = new List<UserGame>();
            Friends = new List<UserFriend>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        // Navigation properties
        public List<Post> Posts { get; set; }
        public List<Review> Reviews { get; set; }

        // Many-to-many with Game
        public List<UserGame> GameLibrary { get; set; }

        // Many-to-many self-referencing for friends
        public List<UserFriend> Friends { get; set; }
    }

    public class UserGame
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int GameId { get; set; }  // just store the IGDB game ID
    }

    // Junction table for User <-> User (friends)
    public class UserFriend
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int FriendId { get; set; }
        public User Friend { get; set; }
    }
}