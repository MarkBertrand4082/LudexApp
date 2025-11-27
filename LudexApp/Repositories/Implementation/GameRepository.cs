// Mark Bertrand
using IGDB;
using IGDB.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;

namespace LudexApp.Repositories.Implementation
{
    public class GameRepository : IGameRepository
    {
        // For a real app, move these to configuration / env vars.
        private static readonly IGDBClient _igdb =
            IGDBClient.CreateWithDefaults(Environment.GetEnvironmentVariable("9cm2gxrs70uz3tsepmq63txsb9grz2"), Environment.GetEnvironmentVariable("t73n320sd26wp6i0ja3bxfn8fml83k"));

        // Expose if you really need the raw client
        public IGDBClient GetIgdb() => _igdb;

        // Cached task for "all games" query
        public Task<IEnumerable<Game>> Games => GetGamesAsync();

        public GameRepository()
        {
        }

        // ----------------------------------------------------
        // Featured games - used on Home Page
        // ----------------------------------------------------
        public async Task<IEnumerable<Game>> GetFeaturedGamesAsync()
        {
            return await _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                query: "fields id, name, cover.url, aggregated_rating; sort aggregated_rating desc; limit 20;");
        }

        // ----------------------------------------------------
        // All games (or a broader list) - used for Library
        // ----------------------------------------------------
        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await GetGamesAsync();
        }

        private static async Task<IEnumerable<Game>> GetGamesAsync()
        {
            // You can expand fields here as needed for Library / Details
            return await _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                query: "fields id, name, cover.url, aggregated_rating, platforms.name, involved_companies; limit 200;");
        }

        // ----------------------------------------------------
        // Search games by name (simple in-memory search)
        // ----------------------------------------------------
        public IEnumerable<GameSummaryViewModel> SearchGames(string name)
        {
            IEnumerable<Game> searchResult =
                from game in Games.Result
                where game.Name != null && game.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                select game;

            var gameSummaries = new List<GameSummaryViewModel>();

            foreach (Game g in searchResult)
            {
                gameSummaries.Add(new GameSummaryViewModel
                {
                    Title = g.Name,
                    GameId = (int)g.Id,
                    AverageRating = g.AggregatedRating
                });
            }

            return gameSummaries;
        }

        // ----------------------------------------------------
        // Search one specific game by name + dev
        // ----------------------------------------------------
        public GameSummaryViewModel SearchSpecificGame(string name, string dev)
        {
            var result = (from game in Games.Result
                          where game.Name != null && game.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                          where game.InvolvedCompanies.Values
                                .SingleOrDefault(item =>
                                    item.Developer == true &&
                                    item.Company.Value.Name.Contains(dev, StringComparison.OrdinalIgnoreCase)) != null
                          select game).FirstOrDefault();

            if (result == null)
            {
                return new GameSummaryViewModel
                {
                    Title = "Unknown Game",
                    Platform = "",
                    GameId = 0
                };
            }

            var platformName = result.Platforms?.Values.FirstOrDefault()?.Name ?? "";

            return new GameSummaryViewModel
            {
                Title = result.Name,
                Platform = platformName,
                GameId = (int)result.Id,
                AverageRating = result.AggregatedRating
            };
        }

        // ----------------------------------------------------
        // Get a single game by IGDB id - used by Details page
        // ----------------------------------------------------
        public async Task<Game?> GetGameByIdAsync(long id)
        {
            var games = await _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                query: $@"fields id, name, summary, cover.url, aggregated_rating,
                                first_release_date, genres.name, platforms.name, screenshots.url;
                          where id = {id}; limit 1;");

            return games.FirstOrDefault();
        }
    }
}
