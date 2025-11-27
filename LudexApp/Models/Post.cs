using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public int ForumId { get; set; }
        public Forum Forum { get; set; }

        // User who created the post
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
