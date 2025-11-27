using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        // Navigation to forum
        public int ForumId { get; set; }
        public Forum Forum { get; set; }

        // Navigation to user who created it
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
