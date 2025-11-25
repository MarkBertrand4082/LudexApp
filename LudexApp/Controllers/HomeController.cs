// Andrew Neto
using IGDB;
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
        private readonly IUserRepository _userRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly IGDBClient _igdb;

        public HomeController(
            IGameRepository gameRepository,
            IUserRepository userRepository,
            ILogger<HomeController> logger,
            IGDBClient igdb)
        {
            _gameRepository = gameRepository;
            _userRepository = userRepository;
            _logger = logger;
            _igdb = igdb;
        }

        // ------------------------------------------------------------------
        // View Home Page -> DisplayFeaturedGames(), DisplayFriends(), DisplayList()
        // ------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel
            {
                IsLoggedIn = User.Identity?.IsAuthenticated ?? false
            };

            // Get Featured Games
            model.FeaturedGames = await _gameRepository.GetFeaturedGamesAsync(_igdb);

            if (model.IsLoggedIn)
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId.HasValue)
                {
                    // Display List
                    model.UserGameList = await _gameRepository.GetUserGameListAsync(currentUserId.Value);

                    // Display Friends
                    model.Friends = await _userRepository.GetFriendsAsync(currentUserId.Value);
                }
                else
                {
                    _logger.LogWarning("User is authenticated but no valid user id was found in claims.");
                }
            }

            // Looks for Views/Home/HomePage.cshtml
            return View("HomePage", model);
        }
        private int? GetCurrentUserId()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idString, out var id))
            {
                return id;
            }

            return null;
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
            return RedirectToAction("Index", "Library");
        }

        [HttpPost]
        public IActionResult NavigateToFriend()
        {
            return RedirectToAction("Index", "Friend");
        }
    }
}
