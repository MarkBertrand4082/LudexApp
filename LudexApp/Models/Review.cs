// Andrew Neto
using IGDB.Models;
using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int GameId { get; set; }

        public int UserId { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(600)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (your teammates will hook these up)
        public Game? Game { get; set; }
        public User? User { get; set; }
    }
}
