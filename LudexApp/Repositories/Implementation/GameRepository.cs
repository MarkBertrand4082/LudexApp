//Mark Bertrand
using IGDB;
using IGDB.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;

namespace LudexApp.Repositories.Implementation
{
    public class GameRepository : IGameRepository
    {
        private readonly IGDBClient _igdb;

        public GameRepository(IGDBClient igdb)
        {
            _igdb = igdb;
        }

        public async Task<IEnumerable<Game>> GetFeaturedGamesAsync()
        {
            return await _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                query: "fields id, name, cover.url, aggregated_rating; sort aggregated_rating desc; limit 4; " +
                "where id = 119171 | " +
                "id = 115289 | " +
                "id = 228456 | " +
                "id = 55189;");
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                query: "fields id, name, cover.url, aggregated_rating, platforms.name; limit 200;");
        }

        public async Task<Game?> GetGameByIdAsync(long id)
        {
            var games = await _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                query: $@"fields id, name, summary, cover.url, aggregated_rating,
                                first_release_date, genres.name, platforms.name;
                          where id = {id}; limit 1;");

            return games.FirstOrDefault();
        }

        public IEnumerable<GameSummaryViewModel> SearchGames(string name)
        {
            // For simplicity we block on the async call - OK for this project.
            var games = _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                $"search \"{name}\"; fields id, name, cover.url, aggregated_rating, platforms.name; limit 50;"
            ).Result;

            var result = new List<GameSummaryViewModel>();

            foreach (var g in games)
            {
                var vm = new GameSummaryViewModel
                {
                    GameId = (int)g.Id,
                    Title = g.Name ?? "Unknown",
                    AverageRating = g.AggregatedRating
                };

                if (g.Platforms != null && g.Platforms.Values.Any())
                {
                    vm.Platform = string.Join(", ",
                        g.Platforms.Values
                            .Where(p => p != null && !string.IsNullOrEmpty(p.Name))
                            .Select(p => p.Name));
                }
                else
                {
                    vm.Platform = "";
                }

                result.Add(vm);
            }

            return result;
        }
        public GameSummaryViewModel SearchSpecificGame(string name, string dev)
        {
            var games = _igdb.QueryAsync<Game>(
                IGDBClient.Endpoints.Games,
                $@"search ""{name}""; 
                   fields id, name, platforms.name, aggregated_rating, involved_companies.company.name, involved_companies.developer;
                   limit 50;"
            ).Result;

            var match = games.FirstOrDefault(g =>
                g.InvolvedCompanies?.Values.Any(ic =>
                    ic.Developer == true &&
                    ic.Company.Value.Name.Contains(dev, StringComparison.OrdinalIgnoreCase)) == true);

            if (match == null)
            {
                return new GameSummaryViewModel
                {
                    GameId = 0,
                    Title = "Unknown Game",
                    Platform = "",
                    AverageRating = null
                };
            }

            return new GameSummaryViewModel
            {
                GameId = (int)match.Id,
                Title = match.Name ?? "Unknown",
                Platform = match.Platforms?.Values.FirstOrDefault()?.Name ?? "",
                AverageRating = match.AggregatedRating
            };
        }
    }
}
