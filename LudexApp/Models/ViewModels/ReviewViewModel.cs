// Andrew Neto
using System.ComponentModel.DataAnnotations;

namespace LudexApp.Models.ViewModels
{
    // For the main Review Page (list of reviews for a game)
    public class ReviewDisplayViewModel
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; } = string.Empty;

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public List<ReviewItemViewModel> Reviews { get; set; } = new();
    }

    // A single review row
    public class ReviewItemViewModel
    {
        public int ReviewId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool CanDelete { get; set; }   // current user owns this review?
        public int GameId { get; set; }
    }

    // For the "Write a Review" form
    public class ReviewCreateViewModel
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; } = string.Empty;

        [Range(1, 10)]
        [Display(Name = "Rating (1–10)")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(600)]
        [Display(Name = "Review")]
        public string Content { get; set; } = string.Empty;
    }
}
