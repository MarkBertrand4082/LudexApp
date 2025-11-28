//Lew Lin
using IGDB.Models;
using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models
{
    public class Forum
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation to posts
        public List<Post> Posts { get; set; } = new List<Post>();

        // Navigation to game
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
