// Andrew Neto
using IGDB.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LudexApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGameRepository _gameRepository;
        public HomeController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        // View Home Page -> DisplayFeaturedGames()

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel
            {
                IsLoggedIn = User.Identity?.IsAuthenticated ?? false
            };

            var igdbGames = await _gameRepository.GetFeaturedGamesAsync();

            foreach (var g in igdbGames)
            {
                var featuredGame = new GameSummaryViewModel
                {
                    GameId = (int)g.Id,
                    Title = g.Name,
                    Platform = g.Platforms?.Values.Any() == true
                        ? string.Join(", ", g.Platforms.Values.Select(p => p.Name))
                        : "",
                    AverageRating = g.Rating
                };

                model.FeaturedGames.Add(featuredGame);
            }

            return View("HomePage", model);
        }

        // ------------------------------------
        // Navigation methods from HomePage UML
        // ------------------------------------

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GoToLogin()
        {
            return RedirectToAction("Login", "User");
        }


        [HttpPost]
        public IActionResult NavigateToGameLibrary()
        {
            return RedirectToAction("Index", "Game");
        }

        [HttpPost]
        public IActionResult NavigateToFriend()
        {
            return RedirectToAction("Friend", "User");
        }
    }
}
