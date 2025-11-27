// Andrew Neto
namespace LudexApp.Models.ViewModels
{
    // For the Game Library (list page)
    public class GameLibraryViewModel
    {
        public string? SearchTerm { get; set; }
        public List<GameSummaryViewModel> Games { get; set; } = new();
    }

    // For the Game Details page
    public class GameDetailViewModel
    {
        public int GameId { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }
        public string? CoverUrl { get; set; }

        public double? AverageRating { get; set; }

        public string Platforms { get; set; } = string.Empty;
        public string Genres { get; set; } = string.Empty;

        public DateTime? ReleaseDate { get; set; }
    }
}
