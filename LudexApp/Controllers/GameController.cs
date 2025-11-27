// Andrew Neto
using IGDB.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LudexApp.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameRepository gameRepository, ILogger<GameController> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm = null)
        {
            var model = new GameLibraryViewModel
            {
                SearchTerm = searchTerm
            };

            // If there's a search term, use your SearchGames helper
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchResults = _gameRepository.SearchGames(searchTerm);
                model.Games = searchResults.ToList();
            }
            else
            {
                // Otherwise show featured or "all" games
                var games = await _gameRepository.GetAllGamesAsync();

                foreach (Game g in games)
                {
                    var vm = new GameSummaryViewModel
                    {
                        GameId = (int)g.Id,
                        Title = g.Name ?? "Unknown",
                        AverageRating = g.AggregatedRating
                    };

                    // Build comma-separated platform list
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

                    model.Games.Add(vm);
                }
            }

            return View("GameLibrary", model); // Views/Game/GameLibrary.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var game = await _gameRepository.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var model = new GameDetailViewModel
            {
                GameId = (int)game.Id,
                Title = game.Name ?? "Unknown Game",
                Summary = game.Summary,
                AverageRating = game.AggregatedRating,
                CoverUrl = game.Cover?.Value?.Url
            };

            if (game.Platforms != null && game.Platforms.Values.Any())
            {
                model.Platforms = string.Join(", ",
                    game.Platforms.Values
                        .Where(p => p != null && !string.IsNullOrEmpty(p.Name))
                        .Select(p => p.Name));
            }

            if (game.Genres != null && game.Genres.Values.Any())
            {
                model.Genres = string.Join(", ",
                    game.Genres.Values
                        .Where(g => g != null && !string.IsNullOrEmpty(g.Name))
                        .Select(g => g.Name));
            }

            if (game.FirstReleaseDate != null)
            {
                model.ReleaseDate = game.FirstReleaseDate.Value.DateTime;
            }

            return View("GameDetails", model); // Views/Game/GameDetails.cshtml
        }
    }
}
