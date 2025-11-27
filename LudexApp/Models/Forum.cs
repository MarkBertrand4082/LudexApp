using IGDB.Models;
using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models
{
    public class Forum
    {
        [Key]
        public int Id { get; set; }

        // Game this forum belongs to
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation to posts
        public List<Post> Posts { get; set; } = new();
    }
}
