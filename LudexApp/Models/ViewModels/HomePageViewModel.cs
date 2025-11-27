// Andrew Neto
namespace LudexApp.Models.ViewModels
{
    public class HomePageViewModel
    {
        // Displays Featured Games
        public List<GameSummaryViewModel> FeaturedGames { get; set; } = new();

        // Displays list of user's games
        public List<GameSummaryViewModel> UserGameList { get; set; } = new();

        // Displays Friends
        public List<FriendSummaryViewModel> Friends { get; set; } = new();

        // Checks whether or not the User is logged in or now (Distinguish between visitor and user)
        public bool IsLoggedIn { get; set; }

        public int? CurrentUserId { get; set; }
    }

    // Data Encapsulation for Game Summary View on Home Page
    public class GameSummaryViewModel
    {
        public int GameId { get; set; }
        public string Title { get; set; }

        public string Platform { get; set; }

        public double? AverageRating { get; set; }

        public string? CoverUrl { get; set; }
    }

    // Data Encapsulation for Friend Summary View on Home Page
    public class FriendSummaryViewModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public int? SharedGamesCount { get; set; }
    }
}
