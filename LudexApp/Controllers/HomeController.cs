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

            foreach (Game g in igdbGames)
            {
                var featuredGame = new GameSummaryViewModel();
                featuredGame.GameId = (int)g.Id;
                featuredGame.Title = g.Name;
                featuredGame.Platform = "";

                foreach (Platform p in g.Platforms.Values)
                {
                    if (p != g.Platforms.Values.Last()) featuredGame.Platform = p.Name + ", ";
                    else featuredGame.Platform = p.Name;
                }

                featuredGame.AverageRating = g.Rating;
            }

            // Looks for Views/Home/HomePage.cshtml
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
        public IActionResult GoToLogin()
        {
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public IActionResult ContinueAsGuest()
        {
            return RedirectToAction(nameof(Index));
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
